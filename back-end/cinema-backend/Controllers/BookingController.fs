namespace CinemaSeatReservationSystem1.Controllers

open System
open Microsoft.AspNetCore.Mvc
open CinemaSeatReservationSystem1.Domain
open CinemaSeatReservationSystem1

[<ApiController>]
[<Route("api")>]
type BookingController () =
    inherit ControllerBase()

    // 1. عرض الأفلام (مع البحث)
    [<HttpGet("movies")>]
    member this.GetMovies([<FromQuery>] search: string) : IActionResult =
        let movies = 
            if String.IsNullOrWhiteSpace(search) then
                Database.getAllMovies()
            else
                Database.searchMovies search
        
        this.Ok(movies) :> IActionResult

    // 2. حجز التذكرة (بالتعديل الجديد Guid ID)
    [<HttpPost("book")>]
    member this.BookTicket(req: BookingRequest) : IActionResult =
        // أولاً: التأكد أن المقعد غير محجوز
        if Database.isSeatTaken req.MovieId req.SeatRow req.SeatNumber then
            this.BadRequest("This seat is already booked.") :> IActionResult
        else
            // ثانياً: جلب بيانات الفيلم لحساب السعر
            match Database.getMovieById req.MovieId with
            | None -> this.NotFound("Movie not found") :> IActionResult
            | Some movie ->
                
                let priceMultiplier = 
                    match req.Category.ToUpper() with
                    | "PLATINUM" -> 2.0m
                    | "GOLD" -> 1.5m
                    | _ -> 1.0m 
                
                let finalPrice = movie.BasePrice * priceMultiplier
                
                // استخدام Guid كما طلبت لحل مشكلة الـ ID
                let ticketId = Guid.NewGuid().ToString() 

                let newTicket = {
                    Id = ticketId 
                    UserId = req.UserId
                    MovieId = req.MovieId
                    SeatRow = req.SeatRow
                    SeatNumber = req.SeatNumber
                    TicketType = req.Category
                    Price = finalPrice
                    IsPaid = true
                    BookingDate = DateTime.Now
                }

                try 
                    Database.saveTicket newTicket
                    this.Ok(newTicket) :> IActionResult
                with
                | ex -> 
                    this.BadRequest("Booking failed: " + ex.Message) :> IActionResult

    // 3. جلب المقاعد المحجوزة لفيلم معين (عشان نرسمها في الفرونت)
    [<HttpGet("movies/{id}/booked-seats")>]
    member this.GetBookedSeats(id: int) : IActionResult =
        try
            let seats = Database.getBookedSeats id
            this.Ok(seats) :> IActionResult
        with
        | ex -> this.BadRequest("Error fetching seats: " + ex.Message) :> IActionResult
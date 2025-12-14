namespace CinemaSeatReservationSystem1.Controllers

open System
open Microsoft.AspNetCore.Mvc
open CinemaSeatReservationSystem1.Domain
open CinemaSeatReservationSystem1

[<ApiController>]
[<Route("api")>]
type BookingController () =
    inherit ControllerBase()

    // 1. DISPLAY MOVIEEES
    [<HttpGet("movies")>]
    member this.GetMovies([<FromQuery>] search: string) : IActionResult =
        let movies = 
            if String.IsNullOrWhiteSpace(search) then
                Database.getAllMovies()
            else
                Database.searchMovies search
        this.Ok(movies) :> IActionResult

    // 2.  BOOK TICKECT
    [<HttpPost("book")>]
    member this.BookTicket(req: BookingRequest) : IActionResult =
        if Database.isSeatTaken req.MovieId req.SeatRow req.SeatNumber then
            this.BadRequest("This seat is already booked.") :> IActionResult
        else
            match Database.getMovieById req.MovieId with
            | None -> this.NotFound("Movie not found") :> IActionResult
            | Some movie ->
                
                let priceMultiplier = 
                    match req.Category.ToUpper() with
                    | "PLATINUM" -> 2.0m
                    | "GOLD" -> 1.5m
                    | _ -> 1.0m 
                
                let finalPrice = movie.BasePrice * priceMultiplier
                let ticketId = Guid.NewGuid().ToString() 

                let newTicket = {
                    Id = ticketId 
                    UserId = req.UserId; MovieId = req.MovieId
                    SeatRow = req.SeatRow; SeatNumber = req.SeatNumber
                    TicketType = req.Category; Price = finalPrice
                    IsPaid = true; BookingDate = DateTime.Now
                }

                try 
                    Database.saveTicket newTicket
                    this.Ok(newTicket) :> IActionResult
                with
                | ex -> this.BadRequest("Booking failed: " + ex.Message) :> IActionResult

    // 3. (Tuple List)
    [<HttpGet("movies/{id}/booked-seats")>]
    member this.GetBookedSeats(id: int) : IActionResult =
        try
            let seats = Database.getBookedSeats id
            this.Ok(seats) :> IActionResult
        with
        | ex -> this.BadRequest("Error fetching seats: " + ex.Message) :> IActionResult

    
    // 4. (Seat Layout Architect Implementation)
    
    
    [<HttpGet("movies/{id}/seat-matrix")>]
    member this.GetSeatMapMatrix(id: int) : IActionResult =
        try
           
            let rows = 8
            let cols = 8
            let seatMatrix = Array2D.create rows cols "Free"

            
            let bookedSeats = Database.getBookedSeats id
            for seat in bookedSeats do
                if seat.SeatRow >= 0 && seat.SeatRow < rows && seat.SeatNumber >= 0 && seat.SeatNumber < cols then
                    seatMatrix.[seat.SeatRow, seat.SeatNumber] <- "Booked"

            //  Jagged Array FOR JSON TO DIPLAY
            
            let jsonReadyMatrix = 
                [| for r in 0 .. rows - 1 -> 
                       [| for c in 0 .. cols - 1 -> seatMatrix.[r, c] |] 
                |]

            // READY MATRIX  إ
            this.Ok(jsonReadyMatrix) :> IActionResult
        with
        | ex -> this.BadRequest("Error generating seat matrix: " + ex.Message) :> IActionResult
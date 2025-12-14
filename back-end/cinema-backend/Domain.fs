namespace CinemaSeatReservationSystem1
open Microsoft.AspNetCore.Http


open System

module Domain =
    
    type User = {
        Id: int
        Username: string
        Email: string
        Role: string
    }

    type Movie = {
        Id: int
        Title: string
        Description: string
        ImageUrl: string
        ShowDate: DateTime
        HallNumber: int
        BasePrice: decimal
    }

    
type MovieUpload() =
    member val Title = "" with get, set
    member val Description = "" with get, set
    member val ShowDate = DateTime.Now with get, set
    member val HallNumber = 0 with get, set
    member val BasePrice = 0M with get, set
    member val Image : IFormFile = null with get, set


    type Ticket = {
        Id: string
        UserId: int
        MovieId: int
        SeatRow: int
        SeatNumber: int
        TicketType: string
        Price: decimal
        IsPaid: bool
        BookingDate: DateTime
    }

    type LoginRequest = {
        Username: string
        Password: string
    }

    type RegisterRequest = {
        Username: string
        Password: string
        Email: string
    }

    type BookingRequest = {
        UserId: int
        MovieId: int
        SeatRow: int
        SeatNumber: int
        Category: string
    }
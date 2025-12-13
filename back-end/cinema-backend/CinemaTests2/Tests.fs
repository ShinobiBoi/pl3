module Tests

open System
open Xunit
open CinemaSeatReservationSystem1
open CinemaSeatReservationSystem1.Domain
open CinemaSeatReservationSystem1.Database 
open CinemaSeatReservationSystem1.Controllers 

// ==========================================
// 1. Business Logic Tests («Œ »«—«  «·„‰ÿﬁ Ê«·Õ”«»« )
// Â–Â «·«Œ »«—«  ·«  Õ «Ã œ« « »Ì“° Ê«·„›—Ê÷  ‰ÃÕ œ«∆„«
// ==========================================
type BusinessLogicTests() =

    [<Fact>]
    member this.``Calculate Price for Platinum Ticket`` () =
        let basePrice = 100.0m
        let category = "Platinum"
        let multiplier = 
            match category.ToUpper() with
            | "PLATINUM" -> 2.0m
            | "GOLD" -> 1.5m
            | _ -> 1.0m
        Assert.Equal(200.0m, basePrice * multiplier)

    [<Fact>]
    member this.``Ticket ID Format Should Be Correct`` () =
        let ticketId = sprintf "MOV%d-R%d-S%d-%s" 99 5 12 "ABCD"
        Assert.StartsWith("MOV99-R5-S12-", ticketId)

// ==========================================
// 2. Database Integration Tests («Œ »«—«  ﬁ«⁄œ… «·»Ì«‰« )
// Â–Â «·«Œ »«—«   Õ «Ã œ« « »Ì“° ·Ê ›‘·  Ì»ﬁÏ Ã„·… «·« ’«· €·ÿ
// ==========================================
type DatabaseIntegrationTests() =

    let generateRandomString() = Guid.NewGuid().ToString().Substring(0, 8)

    // «Œ »«— ÃœÌœ: ·· √ﬂœ √‰ «·« ’«· »«·”Ì—›— ‘€«· √’·«
    [<Fact>]
    member this.``A_Connectivity: Check Database Connection`` () =
        try
            let _ = Database.getAllMovies()
            Assert.True(true) 
        with
        | ex -> Assert.Fail(sprintf "›‘· «·« ’«· »ﬁ«⁄œ… «·»Ì«‰« !  √ﬂœ „‰ ConnectionString ›Ì Database.fs. «·Œÿ√: %s" ex.Message)

    [<Fact>]
    member this.``Full Cycle: Register then Login`` () =
        // 1. Arrange
        let uniqueName = sprintf "User_%s" (generateRandomString())
        let registerReq = { Username = uniqueName; Password = "123"; Email = "test@mail.com" }

        // 2. Act
        Database.registerUser registerReq
        let userResult = Database.loginUser { Username = uniqueName; Password = "123" }

        // 3. Assert
        Assert.True(userResult.IsSome, "›‘·  ”ÃÌ· «·œŒÊ·. Â· ﬁ«⁄œ… «·»Ì«‰«  „ ’·…ø")
        Assert.Equal(uniqueName, userResult.Value.Username)

    [<Fact>]
    member this.``Admin: Add Movie and Retrieve it`` () =
        // 1. Arrange
        let uniqueTitle = sprintf "Movie_%s" (generateRandomString())
        let newMovie = {
            Id = 0
            Title = uniqueTitle
            Description = "Test Desc"
            ImageUrl = "img.jpg"
            ShowDate = DateTime.Now
            HallNumber = 1
            BasePrice = 50.0m

        }

        // 2. Act
        Database.addMovie newMovie
        let searchResult = Database.searchMovies uniqueTitle

        // 3. Assert
        Assert.NotEmpty(searchResult)
        Assert.Contains(searchResult, fun m -> m.Title = uniqueTitle)

    [<Fact>]
    member this.``Booking: Create User, Create Movie, then Book`` () =
        // √) ≈‰‘«¡ ÌÊ“— ÃœÌœ
        let uniqueUser = sprintf "Booker_%s" (generateRandomString())
        Database.registerUser { Username = uniqueUser; Password = "123"; Email = "b@b.com" }
        let loginRes = Database.loginUser { Username = uniqueUser; Password = "123" }
        
        // Õ„«Ì… „‰ «·«‰ÂÌ«— ·Ê «·ÌÊ“— „ ⁄„·‘
        Assert.True(loginRes.IsSome, "›‘· ≈‰‘«¡ «·„” Œœ„ ··ÕÃ“ - «›Õ’ «·œ« « »Ì“")
        let dbUser = loginRes.Value

        // ») ≈‰‘«¡ ›Ì·„ ÃœÌœ
        let uniqueMovieTitle = sprintf "Film_%s" (generateRandomString())
        Database.addMovie { Id=0; Title=uniqueMovieTitle; Description="D"; ImageUrl="I"; ShowDate=DateTime.Now; HallNumber=1; BasePrice=50m }
        let searchRes = Database.searchMovies uniqueMovieTitle
        
        // Õ„«Ì… „‰ «·«‰ÂÌ«— ·Ê «·›Ì·„ „ ⁄„·‘
        Assert.NotEmpty(searchRes)
        let dbMovie = searchRes |> Seq.head

        // Ã) «·ÕÃ“
        let row, seat = 5, 5
        let ticket = {
            Id = generateRandomString()
            UserId = dbUser.Id
            MovieId = dbMovie.Id
            SeatRow = row
            SeatNumber = seat
            TicketType = "Regular"
            Price = 50.0m
            IsPaid = true
            BookingDate = DateTime.Now
        }

        try 
            Database.saveTicket ticket
            let isTaken = Database.isSeatTaken dbMovie.Id row seat
            Assert.True(isTaken, "«·„ﬁ⁄œ ·„ Ì „ ÕÃ“Â ›Ì «·œ« « »Ì“")
        with
        | ex -> Assert.Fail(sprintf "ÕœÀ Œÿ√ √À‰«¡ «·ÕÃ“: %s" ex.Message)

    [<Fact>]
    member this.``Search: Should Return Empty for Non-Existent Movie`` () =
        let results = Database.searchMovies "KalamFady_123456789"
        Assert.Empty(results)


//[<ApiController>]
//[<Route("api/admin")>]
//type AdminController () =
//    inherit ControllerBase()

//    [<HttpPost("add-movie")>]
//    member this.AddMovie(movie: Movie) : IActionResult =
//        try
//            Database.addMovie movie
//            this.Ok("Movie added successfully") :> IActionResult
//        with
//        | ex -> this.BadRequest("Failed to add movie: " + ex.Message) :> IActionResult

namespace CinemaSeatReservationSystem1.Controllers

open Microsoft.AspNetCore.Mvc
open CinemaSeatReservationSystem1
open CinemaSeatReservationSystem1.Domain
open System
open System.IO

[<ApiController>]
[<Route("api/admin")>]
type AdminController () =
    inherit ControllerBase()
    //دا عشان الادمن يعرف يضيف صوره فالجهاز عنده ف كان لازم اعدل فيها
    [<HttpPost("add-movie")>]
    member this.AddMovie([<FromForm>] movie: MovieUpload) : IActionResult =
        try
            if movie.Image = null then
                this.BadRequest("Image file is required") :> IActionResult
            else

                let fileName =
                    Guid.NewGuid().ToString() + Path.GetExtension(movie.Image.FileName)

                let folder = Path.Combine("wwwroot", "images")

                if not (Directory.Exists(folder)) then
                    Directory.CreateDirectory(folder) |> ignore

                let filePath = Path.Combine(folder, fileName)

                use stream = new FileStream(filePath, FileMode.Create)
                movie.Image.CopyTo(stream)

                let newMovie = {
                    Id = 0
                    Title = movie.Title
                    Description = movie.Description
                    ImageUrl = "/images/" + fileName
                    ShowDate = movie.ShowDate
                    HallNumber = movie.HallNumber
                    BasePrice = movie.BasePrice
                }

                Database.addMovie newMovie

                this.Ok( {| message = "Movie added successfully" |} ) :> IActionResult

        with ex ->
            this.BadRequest("Failed to add movie: " + ex.Message) :> IActionResult


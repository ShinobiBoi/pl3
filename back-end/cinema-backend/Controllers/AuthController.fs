namespace CinemaSeatReservationSystem1.Controllers

open Microsoft.AspNetCore.Mvc
open CinemaSeatReservationSystem1.Domain
open CinemaSeatReservationSystem1 

[<ApiController>]
[<Route("api/auth")>]
type AuthController () =
    inherit ControllerBase()

    [<HttpPost("register")>]
    member this.Register(req: RegisterRequest) : IActionResult = 
        // نستقبل نتيجة التسجيل (true = نجاح، false = فشل/مكرر)
        let isSuccess : bool = Database.registerUser req
      
        if isSuccess then
           this.Ok({| message = "User registered successfully" |}) :> IActionResult
        else
           // 409 Conflict: تعني أن المستخدم موجود بالفعل
           this.Conflict({| message = "Username already exists." |}) :> IActionResult   
    
    [<HttpPost("login")>]
    member this.Login(req: LoginRequest) : IActionResult =
        match Database.loginUser req with
        | Some user -> this.Ok(user) :> IActionResult
        | None -> this.Unauthorized("Invalid username or password") :> IActionResult
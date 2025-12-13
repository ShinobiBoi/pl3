namespace CinemaSeatReservationSystem1

open System
open Microsoft.Data.SqlClient
open Dapper
open Domain

module Database =
    
    // جملة الاتصال - تأكد أنها مطابقة لجهازك
    let connectionString = "Server=DESKTOP-QI6H2EA;Database=CinemaDB;Trusted_Connection=true;TrustServerCertificate=true;"

    // --- User Operations ---
    
    // التعديل: الدالة ترجع bool (نجاح/فشل) بدلاً من أن تنهار عند الخطأ
    // هذا يسمح للفرونت إند بعرض رسالة "اسم المستخدم موجود مسبقاً" دون توقف السيرفر
    let registerUser (req: RegisterRequest) : bool =
        try
            using (new SqlConnection(connectionString)) (fun conn ->
                let sql = "INSERT INTO Users (Username, Password, Email, Role) VALUES (@Username, @Password, @Email, 'Customer')"
                conn.Execute(sql, req) |> ignore
            )
            true // نجح التسجيل
        with
        | _ -> 
            false // فشل (غالباً الاسم مكرر)

    let loginUser (req: LoginRequest) =
        using (new SqlConnection(connectionString)) (fun conn ->
            // تحديد الأعمدة المطلوبة فقط لتجنب تعارض البيانات مع الـ Model
            let sql = "SELECT Id, Username, Email, Role FROM Users WHERE Username = @Username AND Password = @Password"
            let user = conn.QuerySingleOrDefault<User>(sql, req)
            if isNull (box user) then None else Some user
        )

    // --- Admin Operations ---
    let addMovie (movie: Movie) =
        using (new SqlConnection(connectionString)) (fun conn ->
            let sql = 
                """
                INSERT INTO Movies (Title, Description, ImageUrl, ShowDate, HallNumber, BasePrice)
                VALUES (@Title, @Description, @ImageUrl, @ShowDate, @HallNumber, @BasePrice)
                """
            conn.Execute(sql, movie) |> ignore
        )

    // --- Movie Operations ---
    let getAllMovies () =
        using (new SqlConnection(connectionString)) (fun conn ->
            let sql = "SELECT * FROM Movies"
            conn.Query<Movie>(sql) |> Seq.toList
        )

    let searchMovies (query: string) =
        using (new SqlConnection(connectionString)) (fun conn ->
            let sql = "SELECT * FROM Movies WHERE Title LIKE @Q OR Description LIKE @Q"
            let searchTerm = "%" + query + "%"
            conn.Query<Movie>(sql, {| Q = searchTerm |}) |> Seq.toList
        )

    let getMovieById (id: int) =
        using (new SqlConnection(connectionString)) (fun conn ->
            let sql = "SELECT * FROM Movies WHERE Id = @Id"
            let movie = conn.QuerySingleOrDefault<Movie>(sql, {| Id = id |})
            if isNull (box movie) then None else Some movie
        )

    // --- Booking Operations ---
    let isSeatTaken (movieId: int) (row: int) (number: int) =
        using (new SqlConnection(connectionString)) (fun conn ->
            let sql = "SELECT COUNT(1) FROM Tickets WHERE MovieId = @MId AND SeatRow = @Row AND SeatNumber = @Num"
            let count = conn.ExecuteScalar<int>(sql, {| MId = movieId; Row = row; Num = number |})
            count > 0
        )

    let saveTicket (ticket: Ticket) =
        using (new SqlConnection(connectionString)) (fun conn ->
            let sql = 
                """
                INSERT INTO Tickets (Id, UserId, MovieId, SeatRow, SeatNumber, TicketType, Price, IsPaid, BookingDate)
                VALUES (@Id, @UserId, @MovieId, @SeatRow, @SeatNumber, @TicketType, @Price, @IsPaid, @BookingDate)
                """
            conn.Execute(sql, ticket) |> ignore
        )

    // دالة لجلب المقاعد المحجوزة فقط (للفرونت إند)
    let getBookedSeats (movieId: int) =
        using (new SqlConnection(connectionString)) (fun conn ->
            let sql = "SELECT SeatRow, SeatNumber FROM Tickets WHERE MovieId = @MId"
            conn.Query(sql, {| MId = movieId |}) |> Seq.toList
        )
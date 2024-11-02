using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NCMS_wasm.Server.Logger;
using NCMS_wasm.Server.Repository;
using NCMS_wasm.Shared;

namespace NCMS_wasm.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HotelManagementController : ControllerBase
    {
        private readonly ILogger<HotelManagementController> _logger;
        private readonly HotelRepository _hotelRepository;
        private readonly GuestRepository _guestRepository;
        private readonly CardRepository _cardRepository;
        private readonly EmailRepository _emailRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;
        private readonly FileLogger _fileLogger;
        private readonly string ModuleName;
        public HotelManagementController(ILogger<HotelManagementController> logger, HotelRepository hotelRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env, GuestRepository guestRepository, CardRepository cardRepository,EmailRepository emailRepository)
        {
            _logger = logger;
            _hotelRepository = hotelRepository;
            _httpContextAccessor = httpContextAccessor;
            _env = env;
            _fileLogger = new FileLogger(configuration);
            ModuleName = "HotelManagementController";
            _guestRepository = guestRepository;
            _cardRepository = cardRepository;
            _emailRepository = emailRepository;    
        }

        [HttpPost("AddAccomodations")]
        public async Task<ActionResult<int>> AddAccomodations(Accomodations info)
        {
            try
            {
                int accId = await _hotelRepository.AddAccomodationAsync(info);
              
                return Ok(accId);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [AddAccomodations]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);

          
                return BadRequest($"Exception occurred while adding accomodation info: {ex.Message}");
            }
        }

        [HttpPost("AddRooms")]
        public async Task<ActionResult<int>> AddRooms(RoomInfo rooms)
        {
            try
            {
                rooms.Thumbnail = rooms.Thumbnail is not null&&!rooms.Thumbnail.StartsWith("http")?SaveImageToDisk(rooms.Thumbnail,true):rooms.Thumbnail;
                rooms.Image = rooms.Image is not null && !rooms.Image.StartsWith("http") ? SaveImageToDisk(rooms.Image, false) : rooms.Image;



                int accId = await _hotelRepository.AddRoomsAsync(rooms);             
                return Ok(accId);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [AddRooms]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);              
                return BadRequest($"Exception occurred while adding room info: {ex.Message}");
            }
        }

        [HttpPost("GetBookingInfo")]
        public async Task<ActionResult<Booking>> GetBookingInfo(Booking booking)
        {
            try
            {              
                var result = await _hotelRepository.GetBookingInfoAsync(booking);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [GetBookingInfo]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);
                return BadRequest($"Exception occurred while adding room info: {ex.Message}");
            }
        }

        [HttpPost("AddBooking")]
        public async Task<ActionResult<string>> AddBooking(Booking booking)
        {
            try
            {
                //Insert Guest
                var guestId = await _guestRepository.InsertGuestAsync(booking.Guests);
                string invoiceNo = await _hotelRepository.InsertBookingAsync(booking, guestId);
                //Insert AccessCard
                foreach (var item in booking.AccessCard)
                {
                    await _cardRepository.InsertAccessCardAsync(item);
                    await _hotelRepository.UpdateRoomStatusAsync(item.RoomNumber, RoomStatus.Occupied, booking.CreatedBy, invoiceNo);
                }

                //InsertBooking


               
                return Ok(invoiceNo);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [AddBooking]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);
                return BadRequest($"Exception occurred while adding booking info: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPost("AddReservation")]
        public async Task<ActionResult<string>> AddReservation(Booking booking)
        {
            try
            {               
                string bookingNo = await _hotelRepository.InsertReservationAsync(booking);

                string name = $"{ToSentenceCase(booking.Guests.FirstName)} {ToSentenceCase(booking.Guests.LastName)}";
                string bookingNumber = bookingNo;
                string checkInDate = booking.Guests.CheckInDate.HasValue
        ? booking.Guests.CheckInDate.Value.ToString("MMMM dd, yyyy")
        : "N/A";

                string checkInTime = booking.Guests.CheckInDate.HasValue
                    ? booking.Guests.CheckInDate.Value.ToString("h:mm tt")
                    : "N/A";

                string checkOutDate = booking.Guests.CheckOutDate.HasValue
                    ? booking.Guests.CheckOutDate.Value.ToString("MMMM dd, yyyy")
                    : "N/A";

                string checkOutTime = booking.Guests.CheckOutDate.HasValue
                    ? booking.Guests.CheckOutDate.Value.ToString("h:mm tt")
                    : "N/A";
                string template = GetEmailtemplate(1);
                string filledTemplate = template.Replace("{{Name}}", name).Replace("{{BookingNumber}}", bookingNumber).Replace("{{CheckInDate}}", checkInDate).Replace("{{CheckInTime}}", checkInTime).Replace("{{CheckOutDate}}", checkOutDate).Replace("{{CheckOutTime}}", checkOutTime);


                var msg = new EmailModel
                {
                    ToAddress = booking.Guests.Email,
                    Subject = "Booking Information",
                    Body = filledTemplate,
                    EmailStatus = EmailStatus.OnQueue
                };
                await _emailRepository.InsertEmailAsync(msg);
                return Ok(bookingNo);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [AddReservation]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);
                return BadRequest($"Exception occurred while adding reservation info: {ex.Message}");
            }
        }

        [HttpPost("UpdateBookingStatus")]
        public async Task<ActionResult<int>> UpdateBookingStatus(GuestsInfo guestsInfo)
        {
            try
            {
                //Insert Guest
                var retVal = await _hotelRepository.UpdateBookingStatusAsync(guestsInfo);                                          

                return Ok(retVal);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [UpdateBookingStatus]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);
                return BadRequest($"Exception occurred while updating booking info: {ex.Message}");
            }
        }

        [HttpPost("UpdateBooking")]
        public async Task<ActionResult<string>> UpdateBooking(Booking booking)
        {
            try
            {
                //Update Guest
                var retVal = await _hotelRepository.UpdateBookingAsync(booking);

                return Ok(retVal);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [UpdateBooking]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);
                return BadRequest($"Exception occurred while updating booking info: {ex.Message}");
            }
        }

        [HttpPost("UpdatedPriceAndStatus")]
        public async Task<ActionResult<int>> UpdatedPriceAndStatus(RoomInfo rooms)
        {
            try
            {          
                int accId = await _hotelRepository.UpdatePriceAndStatusAsync(rooms);              
                return Ok(accId);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [UpdatedPriceAndStatus]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);                
                return BadRequest($"Exception occurred while adding room info: {ex.Message}");
            }
        }

        [HttpPost("AddUpdateInventoryItems")]
        public async Task<ActionResult<int>> AddUpdateInventoryItems(InventoryItems items)
        {
            try
            {
                int accId = await _hotelRepository.AddOrUpdateInventoryAsync(items);
                return Ok(accId);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [AddUpdateInventoryItems]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);
                return BadRequest($"Exception occurred while adding/updating inventory items: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("GetRooms")]
        public async Task<ActionResult<List<RoomInfo>>> GetAllRooms()
        {
            try
            {
                var devices = await _hotelRepository.GetAllRoomsAsync();
                _logger.LogInformation("Rooms retrieved successfully.");
                return Ok(devices);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [GetRooms]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);
             
                return BadRequest($"Exception occurred while retrieving rooms: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("GetAvailableRoomsForGuest")]
        public async Task<ActionResult<List<RoomInfo>>> GetAvailableRoomsForGuest(DateTime dateTime)
        {
            try
            {
                var devices = await _hotelRepository.GetAvailableRoomsForGuestAsync(dateTime);
                _logger.LogInformation("Rooms retrieved successfully.");
                return Ok(devices);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [GetAvailableRoomsForGuest]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);

                return BadRequest($"Exception occurred while retrieving rooms: {ex.Message}");
            }
        }

        [HttpGet("GetDashboardSales")]
        public async Task<ActionResult<DashboardValueSales>> GetDashboardSales(int year)
        {
            try
            {
                var result = await _hotelRepository.GetDashboardSalesByMonthAsync(year);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [GetDashboardSales]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);

                return BadRequest($"Exception occurred while retrieving rooms: {ex.Message}");
            }
        }

        [HttpGet("GetInventoryItems")]
        public async Task<ActionResult<InventoryItems>> GetInventoryItems()
        {
            try
            {
                var result = await _hotelRepository.GetInventoryItemsAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [GetInventoryItems]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);

                return BadRequest($"Exception occurred while retrieving inventory items: {ex.Message}");
            }
        }

        [HttpGet("GetCalendarDisplay")]
        public async Task<ActionResult<List<GuestsInfo>>> GetCalendarDisplay()
        {
            try
            {
                var guests = await _hotelRepository.GetCalendarDisplayAsync();
                
                return Ok(guests);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [GetCalendarDisplay]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);

                return BadRequest($"Exception occurred while retrieving rooms: {ex.Message}");
            }
        }

        [HttpGet("GetRoomNumberExist")]
        public async Task<ActionResult<bool>> GetRoomNumberExist(int roomNumber)
        {
            try
            {
                var result = await _hotelRepository.GetRoomsNumberExistAsync(roomNumber);                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [GetRoomNumberExist]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);
                return BadRequest($"Exception occurred while retrieving rooms: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("GetRoomInfo")]
        public async Task<ActionResult<List<RoomInfo>>> GetRoomInfo()
        {
            try
            {
                var devices = await _hotelRepository.GetAllRoomsInfoAsync();
                _logger.LogInformation("Rooms retrieved successfully.");
                return Ok(devices);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [GetRoomInfo]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);
               
                return BadRequest($"Exception occurred while retrieving rooms: {ex.Message}");
            }
        }

        private string SaveImageToDisk(string base64String, bool isThumbnail)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            string serverAddress = $"{request.Scheme}://{request.Host.Value}";
            string filePath = ""; // Define a variable to hold the file path
            
            // Convert the base64 string back to byte array
            byte[] bytes = Convert.FromBase64String(base64String);

            // Generate a unique filename
            string fileName = Guid.NewGuid().ToString() + ".jpg"; // You can change the extension based on the image type

            // Combine the file path with the file name
            //filePath = Path.Combine("C:\\MenuFlix", fileName); // Modify the path as needed
            string folder = isThumbnail ? "Thumbnail" : "360images";
            string folderPath = Path.Combine(_env.WebRootPath, "images", folder);

            // Check if the directory exists, if not, create it
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            filePath = Path.Combine(folderPath, fileName);
            var serverFilePath = $"{folder}/{fileName}";
            // Write the byte array to the file
            System.IO.File.WriteAllBytes(filePath, bytes);
            
            return serverFilePath;
        }

        private string ToSentenceCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }

        private string GetEmailtemplate(int template)
        {
            string templateContent = String.Empty;
            switch (template)
            {
                case 1:
                    templateContent = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <title>Booking Confirmation</title>
    <style type=""text/css"">
        body, table, td, a {
            font-family: 'Arial', sans-serif;
            font-size: 16px;
            color: #333;
        }

        body {
            margin: 0;
            padding: 0;
            width: 100%;
            height: 100%;
        }

        table {
            border-collapse: collapse;
            width: 100%;
        }

        .container {
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            border: 1px solid #ddd;
            background-color: #f9f9f9;
        }

        .header {
            text-align: center;
            padding: 10px 0;
            background-color: #4CAF50;
            color: white;
        }

        .content {
            padding: 20px;
        }

        .footer {
            text-align: center;
            padding: 10px 0;
            font-size: 12px;
            color: #888;
        }

        @media only screen and (max-width: 600px) {
            .container {
                padding: 15px;
            }

            .content {
                padding: 15px;
            }

            h1 {
                font-size: 24px;
            }

            p {
                font-size: 14px;
            }
        }
    </style>
</head>
<body>
    <table class=""container"">
        <tr>
            <td class=""header"">
                <h1>Booking Confirmation</h1>
            </td>
        </tr>
        <tr>
            <td class=""content"">
                <p>Dear <strong>{{Name}}</strong>,</p>
                <p>Thank you for your booking. Below are the details of your reservation:</p>

                <table width=""100%"">
                    <tr>
                        <td><strong>Booking Number:</strong></td>
                        <td>{{BookingNumber}}</td>
                    </tr>
                    <tr>
                        <td><strong>Check-In Date:</strong></td>
                        <td>{{CheckInDate}} at {{CheckInTime}}</td>
                    </tr>
                    <tr>
                        <td><strong>Check-Out Date:</strong></td>
                        <td>{{CheckOutDate}} at {{CheckOutTime}}</td>
                    </tr>
                </table>

                <p>We look forward to hosting you. If you have any questions, feel free to contact us.</p>

                <p>Best regards,<br>The Booking Team</p>
            </td>
        </tr>
        <tr>
            <td class=""footer"">
                <p>&copy; 2024 Norlu CEDEC Midpoint Hotel. All rights reserved.</p>
            </td>
        </tr>
    </table>
</body>
</html>
";
                    break;
            }
            return templateContent;
        }

    }
}

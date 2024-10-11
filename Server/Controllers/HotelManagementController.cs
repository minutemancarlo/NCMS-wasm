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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;
        private readonly FileLogger _fileLogger;
        private readonly string ModuleName;
        public HotelManagementController(ILogger<HotelManagementController> logger, HotelRepository hotelRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env, GuestRepository guestRepository, CardRepository cardRepository)
        {
            _logger = logger;
            _hotelRepository = hotelRepository;
            _httpContextAccessor = httpContextAccessor;
            _env = env;
            _fileLogger = new FileLogger(configuration);
            ModuleName = "HotelManagementController";
            _guestRepository = guestRepository;
            _cardRepository = cardRepository;
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

        [HttpPost("AddBooking")]
        public async Task<ActionResult<int>> AddBooking(Booking booking)
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
            var serverFilePath = $"{serverAddress}/images/{folder}/{fileName}";
            // Write the byte array to the file
            System.IO.File.WriteAllBytes(filePath, bytes);
            
            return serverFilePath;
        }

    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RestaurantWeb.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace RestaurantWeb.Views.Reservation
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly DiplomdbContext _context;

        public IndexModel(DiplomdbContext context)
        {
            _context = context;
        }

        [BindProperty]
        [Required(ErrorMessage = "Пожалуйста, выберите дату и время бронирования")]
        public DateTime ReservationTime { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Введите количество гостей")]
        [Range(1, 20, ErrorMessage = "Количество гостей должно быть от 1 до 20")]
        public int GuestsNumber { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Выберите стол")]
        public int TableId { get; set; }

        [BindProperty]
        [MaxLength(500)]
        public string SpecialRequests { get; set; }

        public List<Table> AvailableTables { get; set; }

        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Логируем все роли
            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type.EndsWith("claims/role"))
                .Select(c => c.Value)
                .ToList();

            var isClient = User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "4");

            if (isClient)
            {

                // По умолчанию показываем столы без бронирований на сегодня
                ReservationTime = DateTime.Now.AddHours(1).Date.AddHours(19); // Сегодня 19:00
                GuestsNumber = 1;

                await LoadAvailableTablesAsync(ReservationTime, GuestsNumber);

                return Page();
            }


            return Redirect("/Home");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadAvailableTablesAsync(ReservationTime, GuestsNumber);
                return Page();
            }

            // Проверяем, что выбранный стол свободен на указанное время
            bool isTableFree = !await _context.Reservations.AnyAsync(r =>
                r.TableId == TableId &&
                r.ReservationTime == ReservationTime &&
                r.Status == "confirmed");

            if (!isTableFree)
            {
                ErrorMessage = "Выбранный стол уже забронирован на это время. Пожалуйста, выберите другой стол или время.";
                await LoadAvailableTablesAsync(ReservationTime, GuestsNumber);
                return Page();
            }

            // Получаем client_id по текущему пользователю (если есть)
            int? clientId = null;
            if (User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.FindFirst("user_id")?.Value ?? "0");
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
                if (client != null)
                    clientId = client.ClientId;
            }

            // Создаем бронирование
            var reservation = new Data.Reservation
            {
                ReservationTime = ReservationTime,
                GuestsNumber = GuestsNumber,
                TableId = TableId,
                Status = "confirmed",
                SpecialRequests = SpecialRequests,
                ClientId = clientId
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            SuccessMessage = "Спасибо! Ваш стол успешно забронирован.";
            // Очистим форму
            ModelState.Clear();
            await LoadAvailableTablesAsync(ReservationTime, GuestsNumber);

            return Page();
        }

        private async Task LoadAvailableTablesAsync(DateTime reservationTime, int guestsNumber)
        {
            // Загружаем столы, которые подходят по вместимости и не заняты на выбранное время
            var reservedTableIds = await _context.Reservations
                .Where(r => r.ReservationTime == reservationTime && r.Status == "confirmed")
                .Select(r => r.TableId)
                .ToListAsync();

            AvailableTables = await _context.Tables
                .Where(t => t.Capacity >= guestsNumber && !reservedTableIds.Contains(t.TableId))
                .OrderBy(t => t.Capacity)
                .ToListAsync();
        }
        
    }
}

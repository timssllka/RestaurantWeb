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
        [Required(ErrorMessage = "����������, �������� ���� � ����� ������������")]
        public DateTime ReservationTime { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "������� ���������� ������")]
        [Range(1, 20, ErrorMessage = "���������� ������ ������ ���� �� 1 �� 20")]
        public int GuestsNumber { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "�������� ����")]
        public int TableId { get; set; }

        [BindProperty]
        [MaxLength(500)]
        public string SpecialRequests { get; set; }

        public List<Table> AvailableTables { get; set; }

        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // �������� ��� ����
            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type.EndsWith("claims/role"))
                .Select(c => c.Value)
                .ToList();

            var isClient = User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "4");

            if (isClient)
            {

                // �� ��������� ���������� ����� ��� ������������ �� �������
                ReservationTime = DateTime.Now.AddHours(1).Date.AddHours(19); // ������� 19:00
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

            // ���������, ��� ��������� ���� �������� �� ��������� �����
            bool isTableFree = !await _context.Reservations.AnyAsync(r =>
                r.TableId == TableId &&
                r.ReservationTime == ReservationTime &&
                r.Status == "confirmed");

            if (!isTableFree)
            {
                ErrorMessage = "��������� ���� ��� ������������ �� ��� �����. ����������, �������� ������ ���� ��� �����.";
                await LoadAvailableTablesAsync(ReservationTime, GuestsNumber);
                return Page();
            }

            // �������� client_id �� �������� ������������ (���� ����)
            int? clientId = null;
            if (User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.FindFirst("user_id")?.Value ?? "0");
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
                if (client != null)
                    clientId = client.ClientId;
            }

            // ������� ������������
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

            SuccessMessage = "�������! ��� ���� ������� ������������.";
            // ������� �����
            ModelState.Clear();
            await LoadAvailableTablesAsync(ReservationTime, GuestsNumber);

            return Page();
        }

        private async Task LoadAvailableTablesAsync(DateTime reservationTime, int guestsNumber)
        {
            // ��������� �����, ������� �������� �� ����������� � �� ������ �� ��������� �����
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

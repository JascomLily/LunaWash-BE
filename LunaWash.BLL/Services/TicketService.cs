using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LunaWash.BLL.Interfaces;

namespace LunaWash.BLL.Services
{
    public class TicketService : ITicketService
    {
        private static List<TicketDto> _tickets = new List<TicketDto>
        {
            new TicketDto { Id = "1", BranchName = "Linh Đông", Category = "Sự cố thiết bị", Title = "Trạm 1 bị kẹt vòi phun áp lực", Description = "Cần kỹ thuật kiểm tra gấp, ảnh hưởng tiến độ...", RequesterName = "Nguyễn Văn A", Time = "10:30 Hôm nay", Status = "Khẩn cấp" },
            new TicketDto { Id = "2", BranchName = "Quận 1", Category = "Yêu cầu phê duyệt", Title = "Đề xuất khuyến mãi đặc biệt khai trương", Description = "Giảm giá 30% cho khách hàng mới...", RequesterName = "Lê Thị B", Time = "08:15 Hôm nay", Status = "Chờ duyệt" },
            new TicketDto { Id = "3", BranchName = "Tân Bình", Category = "Nhân sự", Title = "Báo cáo nghỉ phép đột xuất kỹ thuật", Description = "Anh Hoàng xin nghỉ vì lý do gia đình...", RequesterName = "Trần Văn C", Time = "Hôm qua 16:45", Status = "Đã xử lý" },
            new TicketDto { Id = "4", BranchName = "Quận 7", Category = "Khác", Title = "Báo cáo tồn kho hóa chất", Description = "Lượng hóa chất loại A sắp hết...", RequesterName = "Phạm Minh M", Time = "2 ngày trước", Status = "Đã từ chối" }
        };

        public async Task<IEnumerable<TicketDto>> GetAllTicketsAsync()
        {
            return await Task.FromResult(_tickets);
        }

        public async Task<bool> UpdateTicketStatusAsync(string id, string status)
        {
            var ticket = _tickets.FirstOrDefault(t => t.Id == id);
            if (ticket == null) return await Task.FromResult(false);

            ticket.Status = status;
            return await Task.FromResult(true);
        }
    }
}

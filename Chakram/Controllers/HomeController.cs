using Chakram.Data;
using Chakram.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Threading.Tasks;

using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.IO;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection.Metadata;
using iTextSharp.text.pdf;
namespace Chakram.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<IActionResult> Index(
    string searchQuery,
    string sortOrder = "date_desc",
    DateTime? startDate = null,
    DateTime? endDate = null)
        {
            ViewData["DateSortParam"] = sortOrder == "date_asc" ? "date_desc" : "date_asc";
            ViewData["CurrentFilter"] = searchQuery;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");

            // Получаем базовый запрос
            var baseQuery = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.EmployeeRates)
                .Include(e => e.WorkHours);

            // Получаем общее количество записей
            int totalCount = await baseQuery.CountAsync();

            // Применяем фильтры
            var filteredQuery = baseQuery.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                filteredQuery = filteredQuery.Where(e =>
                    EF.Functions.Like(e.FirstName, $"%{searchQuery}%") ||
                    EF.Functions.Like(e.LastName, $"%{searchQuery}%") ||
                    EF.Functions.Like(e.MiddleName ?? "", $"%{searchQuery}%"));
            }

            if (startDate.HasValue)
            {
                filteredQuery = filteredQuery.Where(e => e.HireDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                filteredQuery = filteredQuery.Where(e => e.HireDate <= endDate.Value);
            }

            // Получаем количество отфильтрованных записей
            int filteredCount = await filteredQuery.CountAsync();

            // Применяем сортировку
            var sortedQuery = sortOrder switch
            {
                "date_asc" => filteredQuery.OrderBy(e => e.HireDate),
                "date_desc" => filteredQuery.OrderByDescending(e => e.HireDate),
                _ => filteredQuery.OrderByDescending(e => e.HireDate)
            };

            // Передаем данные в представление
            ViewBag.TotalCount = totalCount;
            ViewBag.FilteredCount = filteredCount;

            return View(await sortedQuery.ToListAsync());
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            try
            {
                ViewBag.Departments = await _context.Departments.ToListAsync();
                ViewBag.Positions = await _context.Positions.ToListAsync();
                return View(new EmployeeInputModel());
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                return RedirectToAction("Index")
                   ;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(EmployeeInputModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Departments = await _context.Departments.ToListAsync();
                    ViewBag.Positions = await _context.Positions.ToListAsync();
                    return View(model);
                }

                var employee = new Employee
                {
                    LastName = model.LastName,
                    FirstName = model.FirstName,
                    MiddleName = model.MiddleName,
                    PositionId = model.PositionId,
                    DepartmentId = model.DepartmentId,
                    HireDate = model.HireDate
                };

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                // Добавляем ставку сотрудника
                if (model.Rate > 0)
                {
                    _context.EmployeeRates.Add(new EmployeeRate
                    {
                        EmployeeId = employee.EmployeeId,
                        Rate = model.Rate,
                        EffectiveFrom = DateTime.Today
                    });
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index");
                   
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Ошибка при сохранении в базу данных. Проверьте введенные данные.");
                ViewBag.Departments = await _context.Departments.ToListAsync();
                ViewBag.Positions = await _context.Positions.ToListAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Произошла непредвиденная ошибка");
                ViewBag.Departments = await _context.Departments.ToListAsync();
                ViewBag.Positions = await _context.Positions.ToListAsync();
                return View(model);
            }
        }
        // Редактирование сотрудника
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.EmployeeRates)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            ViewBag.Departments = await _context.Departments.ToListAsync();
            ViewBag.Positions = await _context.Positions.ToListAsync();
            ViewBag.CurrentRate = employee.EmployeeRates?
                .OrderByDescending(r => r.EffectiveFrom)
                .FirstOrDefault()?.Rate;

            return View(employee);
        }

        // POST: Home/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee model, decimal rate)
        {
            try
            {
             

                if (id != model.EmployeeId) return NotFound();

                var employee = await _context.Employees
                    .Include(e => e.EmployeeRates)
                    .FirstOrDefaultAsync(e => e.EmployeeId == id);

               

                // Обновляем основные поля
                employee.LastName = model.LastName;
                employee.FirstName = model.FirstName;
                employee.MiddleName = model.MiddleName;
                employee.PositionId = model.PositionId;
                employee.DepartmentId = model.DepartmentId;
                employee.HireDate = model.HireDate;

                // Обновление ставки
                var currentRate = employee.EmployeeRates?
                    .OrderByDescending(r => r.EffectiveFrom)
                    .FirstOrDefault();

                if (currentRate == null || currentRate.Rate != rate)
                {
                  
                    _context.EmployeeRates.Add(new EmployeeRate
                    {
                        EmployeeId = id,
                        Rate = rate,
                        EffectiveFrom = DateTime.Now
                    });
                }

                await _context.SaveChangesAsync();
             
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
             
                ModelState.AddModelError("", "Произошла ошибка при сохранении");
                ViewBag.Departments = await _context.Departments.ToListAsync();
                ViewBag.Positions = await _context.Positions.ToListAsync();
                return View(model);
            }
        }

        // Удаление сотрудника
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.EmployeeRates)
                .Include(e => e.WorkHours)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee != null)
            {
                // Remove related records first
                _context.EmployeeRates.RemoveRange(employee.EmployeeRates);
                _context.WorkHours.RemoveRange(employee.WorkHours);

                // Then remove the employee
                _context.Employees.Remove(employee);

                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
        public class EmployeeInputModel
        {
            public int EmployeeId { get; set; }

            [Required(ErrorMessage = "Фамилия обязательна")]
            [StringLength(50, ErrorMessage = "Не более 50 символов")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Имя обязательно")]
            [StringLength(50, ErrorMessage = "Не более 50 символов")]
            public string FirstName { get; set; }

            [StringLength(50, ErrorMessage = "Не более 50 символов")]
            public string MiddleName { get; set; }

            [Required(ErrorMessage = "Должность обязательна")]
            public int PositionId { get; set; }

            [Required(ErrorMessage = "Отдел обязателен")]
            public int DepartmentId { get; set; }

            [Required(ErrorMessage = "Дата приёма обязательна")]
            [DataType(DataType.Date)]
            public DateTime HireDate { get; set; }

            [Required(ErrorMessage = "Ставка обязательна")]
            [Range(0.01, 100000, ErrorMessage = "Должна быть между 0.01 и 100000")]
            public decimal Rate { get; set; }
        }
    }
}
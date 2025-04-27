using Chakram.Data;
using Chakram.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Chakram.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.EmployeeRates)
                .Include(e => e.WorkHours)
                .ToListAsync();

            return View(employees);
        }

        public async Task<IActionResult> Add()
        {
            ViewBag.Departments = await _context.Departments.ToListAsync();
            ViewBag.Positions = await _context.Positions.ToListAsync();
            return View(new EmployeeInputModel()); // Используем EmployeeInputModel
        }

        [HttpPost]
        public async Task<IActionResult> Add(EmployeeInputModel model)
        {
            if (ModelState.IsValid)
            {
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

                _context.EmployeeRates.Add(new EmployeeRate
                {
                    EmployeeId = employee.EmployeeId,
                    Rate = model.Rate,
                    EffectiveFrom = DateTime.Today
                });
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewBag.Departments = await _context.Departments.ToListAsync();
            ViewBag.Positions = await _context.Positions.ToListAsync();
            return View(model);
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
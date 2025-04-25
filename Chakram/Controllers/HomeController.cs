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
                return View(new EmployeeInputModel());
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

        // �������������� ����������
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.EmployeeRates) // ��������������, ��� EmployeeRates �������� ������
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            ViewBag.Departments = await _context.Departments.ToListAsync();
            ViewBag.Positions = await _context.Positions.ToListAsync();
            ViewBag.CurrentRate = employee.EmployeeRates?.OrderByDescending(r => r.EffectiveFrom).FirstOrDefault()?.Rate;

            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Update(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(employee);
        }

        // �������� ����������
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
    public class EmployeeInputModel
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "������� �����������")]
        [StringLength(50, ErrorMessage = "�� ����� 50 ��������")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "��� �����������")]
        [StringLength(50, ErrorMessage = "�� ����� 50 ��������")]
        public string FirstName { get; set; }

        [StringLength(50, ErrorMessage = "�� ����� 50 ��������")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "��������� �����������")]
        public int PositionId { get; set; }

        [Required(ErrorMessage = "����� ����������")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "���� ����� �����������")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        [Required(ErrorMessage = "������ �����������")]
        [Range(0.01, 100000, ErrorMessage = "������ ���� ����� 0.01 � 100000")]
        public decimal Rate { get; set; }
    }
}
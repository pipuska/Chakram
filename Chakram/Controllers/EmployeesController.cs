using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chakram.Controllers
{
    public class EmployeesController : Controller
    {



        public IActionResult Index()
        {
            return View(); // Главная страница со списком
        }

        public IActionResult Add()
        {
            return View(); // Страница добавления
        }

        public IActionResult Edit(int id)
        {
            ViewBag.EmployeeId = id;
            return View(); // Страница редактирования
        }

        public IActionResult TrackHours()
        {
            return View(); // Страница учета часов
        }


        // GET: EmployeesController
  

        // GET: EmployeesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EmployeesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmployeesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EmployeesController/Edit/5
  

        // POST: EmployeesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EmployeesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EmployeesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}


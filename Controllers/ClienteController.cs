using Microsoft.AspNetCore.Mvc;
using tarea3_aplicaciones_web.Data;
using tarea3_aplicaciones_web.Models;

namespace tarea3_aplicaciones_web.Controllers;

public class ClienteController : Controller
{
    private AppDbContext db;

    private List<string> ciudades = new List<string>
    {
        "Ambato", "Cuenca", "Esmeraldas", "Guayaquil", "Ibarra", "Loja",
        "Machala", "Manta", "Portoviejo", "Quito", "Riobamba", "Santo Domingo"
    };

    public ClienteController(AppDbContext context)
    {
        db = context;
    }

    public IActionResult Index(string? buscar)
    {
        List<Cliente> lista;

        if (string.IsNullOrEmpty(buscar))
        {
            lista = db.Clientes.OrderBy(c => c.Apellidos).ToList();
        }
        else
        {
            lista = db.Clientes
                .Where(c => c.Nombres!.Contains(buscar) || c.Apellidos!.Contains(buscar) || c.Cedula!.Contains(buscar))
                .OrderBy(c => c.Apellidos)
                .ToList();
        }

        ViewBag.Buscar = buscar;
        return View(lista);
    }

    public IActionResult Ver(int id)
    {
        Cliente? cliente = db.Clientes.Find(id);
        if (cliente == null)
        {
            return NotFound();
        }

        return View(cliente);
    }

    public IActionResult Nuevo()
    {
        ViewBag.Ciudades = ciudades;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Nuevo(Cliente cliente)
    {
        Validar(cliente);

        if (!ModelState.IsValid)
        {
            ViewBag.Ciudades = ciudades;
            return View(cliente);
        }

        db.Clientes.Add(cliente);
        db.SaveChanges();

        TempData["exito"] = "El cliente " + cliente.Nombres + " " + cliente.Apellidos + " se registró con éxito";
        return RedirectToAction("Index");
    }

    public IActionResult Editar(int id)
    {
        Cliente? cliente = db.Clientes.Find(id);
        if (cliente == null)
        {
            return NotFound();
        }

        ViewBag.Ciudades = ciudades;
        return View(cliente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(Cliente cliente)
    {
        Cliente? clienteGuardado = db.Clientes.Find(cliente.IdCliente);
        if (clienteGuardado == null)
        {
            return NotFound();
        }

        Validar(cliente);

        if (!ModelState.IsValid)
        {
            ViewBag.Ciudades = ciudades;
            return View(cliente);
        }

        clienteGuardado.Nombres = cliente.Nombres;
        clienteGuardado.Apellidos = cliente.Apellidos;
        clienteGuardado.Cedula = cliente.Cedula;
        clienteGuardado.Email = cliente.Email;
        clienteGuardado.Celular = cliente.Celular;
        clienteGuardado.FechaNacimiento = cliente.FechaNacimiento;
        clienteGuardado.Sexo = cliente.Sexo;
        clienteGuardado.Direccion = cliente.Direccion;
        clienteGuardado.Ciudad = cliente.Ciudad;
        clienteGuardado.CupoCredito = cliente.CupoCredito;
        db.SaveChanges();

        TempData["exito"] = "Los datos de " + cliente.Nombres + " " + cliente.Apellidos + " se actualizaron";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Eliminar(int id)
    {
        Cliente? cliente = db.Clientes.Find(id);
        if (cliente != null)
        {
            db.Clientes.Remove(cliente);
            db.SaveChanges();
            TempData["exito"] = "Se eliminó el cliente " + cliente.Nombres + " " + cliente.Apellidos;
        }

        return RedirectToAction("Index");
    }

    private void Validar(Cliente cliente)
    {
        if (!string.IsNullOrEmpty(cliente.Nombres) && !SoloLetras(cliente.Nombres))
        {
            ModelState.AddModelError("Nombres", "Los nombres solo pueden tener letras");
        }

        if (!string.IsNullOrEmpty(cliente.Apellidos) && !SoloLetras(cliente.Apellidos))
        {
            ModelState.AddModelError("Apellidos", "Los apellidos solo pueden tener letras");
        }

        if (!string.IsNullOrEmpty(cliente.Cedula))
        {
            if (!CedulaValida(cliente.Cedula))
            {
                ModelState.AddModelError("Cedula", "La cédula no es válida");
            }
            else if (db.Clientes.Any(c => c.Cedula == cliente.Cedula && c.IdCliente != cliente.IdCliente))
            {
                ModelState.AddModelError("Cedula", "Ya existe un cliente con esta cédula");
            }
        }

        if (!string.IsNullOrEmpty(cliente.Celular))
        {
            if (cliente.Celular.Length != 10 || !cliente.Celular.StartsWith("09") || !SoloNumeros(cliente.Celular))
            {
                ModelState.AddModelError("Celular", "El celular debe tener 10 números y empezar con 09");
            }
        }

        if (cliente.FechaNacimiento != null)
        {
            DateTime fecha = cliente.FechaNacimiento.Value.Date;
            int edad = DateTime.Today.Year - fecha.Year;
            if (fecha > DateTime.Today.AddYears(-edad))
            {
                edad--;
            }

            if (fecha > DateTime.Today)
            {
                ModelState.AddModelError("FechaNacimiento", "La fecha de nacimiento no puede ser futura");
            }
            else if (edad < 18)
            {
                ModelState.AddModelError("FechaNacimiento", "El cliente tiene que ser mayor de edad (18 años o más)");
            }
        }

        if (!string.IsNullOrEmpty(cliente.Sexo) && cliente.Sexo != "Masculino" && cliente.Sexo != "Femenino" && cliente.Sexo != "Otro")
        {
            ModelState.AddModelError("Sexo", "Seleccione una opción válida");
        }

        if (!string.IsNullOrEmpty(cliente.Ciudad) && !ciudades.Contains(cliente.Ciudad))
        {
            ModelState.AddModelError("Ciudad", "Seleccione una ciudad de la lista");
        }

        if (!string.IsNullOrEmpty(cliente.Direccion) && cliente.Direccion.Trim().Length < 5)
        {
            ModelState.AddModelError("Direccion", "La dirección es muy corta");
        }
    }

    private bool CedulaValida(string cedula)
    {
        if (cedula.Length != 10 || !SoloNumeros(cedula))
        {
            return false;
        }

        int provincia = int.Parse(cedula.Substring(0, 2));
        if (provincia < 1 || provincia > 24)
        {
            return false;
        }

        int tercerDigito = int.Parse(cedula.Substring(2, 1));
        if (tercerDigito >= 6)
        {
            return false;
        }

        int suma = 0;
        for (int i = 0; i < 9; i++)
        {
            int numero = int.Parse(cedula[i].ToString());
            if (i % 2 == 0)
            {
                numero = numero * 2;
                if (numero > 9)
                {
                    numero = numero - 9;
                }
            }
            suma = suma + numero;
        }

        int verificador = (10 - suma % 10) % 10;
        int ultimoDigito = int.Parse(cedula[9].ToString());

        return verificador == ultimoDigito;
    }

    private bool SoloNumeros(string texto)
    {
        foreach (char c in texto)
        {
            if (c < '0' || c > '9')
            {
                return false;
            }
        }
        return true;
    }

    private bool SoloLetras(string texto)
    {
        foreach (char c in texto)
        {
            if (!char.IsLetter(c) && c != ' ')
            {
                return false;
            }
        }
        return true;
    }
}

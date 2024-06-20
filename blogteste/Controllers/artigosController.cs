using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using blogteste.Data;
using blogteste.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace blogteste.Controllers
{
    [Authorize]
    public class artigosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public artigosController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: artigos
        public async Task<IActionResult> Index()
        {
            return View(await _context.artigos.ToListAsync());
        }

        // GET: artigos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artigos = await _context.artigos
                .FirstOrDefaultAsync(m => m.id == id);
            if (artigos == null)
            {
                return NotFound();
            }

            return View(artigos);
        }

        // GET: artigos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: artigos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,titulo,conteudo,autor,data,IsAccessible")] artigos artigos, IFormFile imagem)
        {
            if (ModelState.IsValid)
            {
                if (imagem != null)
                {
                    // Save the image to wwwroot/images
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imagem.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imagem.CopyToAsync(fileStream);
                    }
                    artigos.imagem = uniqueFileName; // Save the image file name to the database
                }

                _context.Add(artigos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(artigos);
        }

        // GET: artigos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artigos = await _context.artigos.FindAsync(id);
            if (artigos == null)
            {
                return NotFound();
            }

            // Verifica se o usuário autenticado é o autor do post
            if (artigos.autor != User.Identity.Name)
            {
                return Forbid();
            }

            return View(artigos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,titulo,conteudo,autor,data")] artigos artigos, IFormFile imagem)
        {
            if (id != artigos.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingArtigo = await _context.artigos.FindAsync(id);
                    if (existingArtigo == null)
                    {
                        return NotFound();
                    }

                    // Atualizar apenas os campos editados
                    existingArtigo.titulo = artigos.titulo;
                    existingArtigo.conteudo = artigos.conteudo;
                    existingArtigo.autor = artigos.autor;
                    existingArtigo.data = artigos.data;

                    if (imagem != null)
                    {
                        // Delete the old image
                        if (!string.IsNullOrEmpty(existingArtigo.imagem))
                        {
                            string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", existingArtigo.imagem);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Save the new image
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imagem.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imagem.CopyToAsync(fileStream);
                        }
                        existingArtigo.imagem = uniqueFileName; // Update the image file name in the database
                    }

                    _context.Update(existingArtigo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!artigosExists(artigos.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(artigos);
        }

        private bool artigosExists(int id)
        {
            return _context.artigos.Any(e => e.id == id);
        }


        // GET: artigos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artigos = await _context.artigos
                .FirstOrDefaultAsync(m => m.id == id);
            if (artigos == null)
            {
                return NotFound();
            }

            // Verifica se o usuário autenticado é o autor do post
            if (artigos.autor != User.Identity.Name)
            {
                return Forbid();
            }

            return View(artigos);
        }

        // POST: artigos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var artigos = await _context.artigos.FindAsync(id);

            // Verifica se o usuário autenticado é o autor do post
            if (artigos.autor != User.Identity.Name)
            {
                return Forbid();
            }

            // Delete the image from the wwwroot/images folder
            if (!string.IsNullOrEmpty(artigos.imagem))
            {
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", artigos.imagem);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.artigos.Remove(artigos);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

     
    }
}

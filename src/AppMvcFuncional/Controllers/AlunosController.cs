﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppMvcFuncional.Data;
using AppMvcFuncional.Models;
using Microsoft.AspNetCore.Authorization;

namespace AppMvcFuncional.Controllers
{
    [Authorize] //precisa estar logado (utilizando identity)
    [Route("meus-alunos")]
    public class AlunosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AlunosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous] //permite anonimos no index
        public async Task<IActionResult> Index()
        {
            //ViewBag.Sucesso = "Listagem bem sucedida!"; --A viewbag sobrevive apenas uma requisição

            return _context.Aluno != null ?
                View(await _context.Aluno.ToListAsync()):
                Problem("Entity set 'ApplicationDbContext.Aluno' is null");
        }

        [Route("{id:int}/detalhes")]
        public async Task<IActionResult> Details(int id)
        {
            if (_context.Aluno == null)
            {
                return NotFound();
            }

            var aluno = await _context.Aluno
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aluno == null)
            {
                return NotFound();
            }

            return View(aluno);
        }

        [Route("novo-aluno")]
        public IActionResult Create()
        {
            return View();
        }

       
        [HttpPost("novo-aluno")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,DataNascimento,Email, EmailConfirmacao, Avaliacao,Ativo")] Aluno aluno)
        {
            if (ModelState.IsValid)
            {
                _context.Add(aluno);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(aluno);
        }

        [Route("editar/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (_context.Aluno == null)
            {
                return NotFound();
            }

            var aluno = await _context.Aluno.FindAsync(id);
            if (aluno == null)
            {
                return NotFound();
            }
            return View(aluno);
        }

        [HttpPost("editar/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,DataNascimento,Email,Avaliacao,Ativo")] Aluno aluno)
        {
            if (id != aluno.Id)
            {
                return NotFound();
            }

            ModelState.Remove("EmailConfirmacao"); //Se não dá erro por não constar o emailconfirmação na edição

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aluno);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlunoExists(aluno.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                TempData["Sucesso"] = "Aluno editado com sucesso!"; //Sobrevive a mais de uma requisição, diferente da viewbag

                return RedirectToAction(nameof(Index));
            }
            return View(aluno);
        }

        [Route("excluir/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Aluno == null)
            {
                return NotFound();
            }

            var aluno = await _context.Aluno
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aluno == null)
            {
                return NotFound();
            }

            return View(aluno);
        }

        [HttpPost("excluir/{id:int}")] 
        [ActionName("Delete")] //Para entender que é a mesma action
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if(_context.Aluno == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Aluno' is null");
            }
            
            var aluno = await _context.Aluno.FindAsync(id);
            if (aluno != null)
            {
                _context.Aluno.Remove(aluno);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlunoExists(int id)
        {
            return _context.Aluno.Any(e => e.Id == id);
        }
    }
}

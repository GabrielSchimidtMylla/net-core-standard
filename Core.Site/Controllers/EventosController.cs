using Core.Application.Interfaces;
using Core.Application.ViewModels;
using Core.Domain.Core.Bus;
using Core.Domain.Core.Notifications;
using Core.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Core.Site.Controllers
{
  [Authorize]
  public class EventosController : BaseController
  {
    private readonly IEventoServices _eventoServices;

    public EventosController(IEventoServices eventoServices,
                             IDomainNotificationHandler<DomainNotification> notifications,
                             IUser user,
                             IBus bus)
      : base(notifications, user, bus)
    {
      _eventoServices = eventoServices;
    }

    // GET: Eventos
    public IActionResult Index()
    {
      var eventos = _eventoServices.ObterTodos();
      return View(eventos);
    }

    // GET: Eventos/Details/5
    public IActionResult Details(Guid? id)
    {
      if (id == null)
        return NotFound();

      var eventoViewModel = _eventoServices.ObterPorId(id.Value);
      if (eventoViewModel == null)
        return NotFound();

      return View(eventoViewModel);
    }

    // GET: Eventos/Create
    public IActionResult Create()
    {
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(EventoViewModel eventoViewModel)
    {
      if (!ModelState.IsValid) return View(eventoViewModel);

      eventoViewModel.OrganizadorId = OrganizadorId;
      _eventoServices.Registrar(eventoViewModel);

      ViewBag.Retorno = OperacaoValida() ? "success, Evento registrado com sucesso"
                                         : "error, Ocorreu algum problema verifique as mensagens";
      return View(eventoViewModel);
    }

    // GET: Eventos/Edit/5
    public IActionResult Edit(Guid? id)
    {
      if (id == null)
        return NotFound();

      var eventoViewModel = _eventoServices.ObterPorId(id.Value);
      if (eventoViewModel == null)
        return NotFound();
      return View(eventoViewModel);
    }

    // POST: Eventos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(EventoViewModel eventoViewModel)
    {
      if (!ModelState.IsValid) return View(eventoViewModel);
      _eventoServices.Atualizar(eventoViewModel);

      ViewBag.Retorno = OperacaoValida() ? "success, Evento atualizado com sucesso"
                                         : "error, Ocorreu algum problema verifique as mensagens";

      return View(eventoViewModel);
    }

    // GET: Eventos/Delete/5
    public IActionResult Delete(Guid? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var eventoViewModel = _eventoServices.ObterPorId(id.Value);
      if (eventoViewModel == null)
      {
        return NotFound();
      }

      return View(eventoViewModel);
    }

    // POST: Eventos/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(Guid id)
    {
      _eventoServices.Excluir(id);

      ViewBag.Retorno = OperacaoValida() ? "success, Evento removido com sucesso"
                                         : "error, Ocorreu algum problema verifique as mensagens";

      return RedirectToAction("Index");
    }

    public IActionResult IncluirEndereco(Guid? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var eventoViewModel = _eventoServices.ObterPorId(id.Value);
      return PartialView("_IncluirEndereco", eventoViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult IncluirEndereco(EventoViewModel eventoViewModel)
    {
      ModelState.Clear();
      eventoViewModel.Endereco.EventoId = eventoViewModel.Id;
      _eventoServices.AdicionarEndereco(eventoViewModel.Endereco);

      if (OperacaoValida())
      {
        string url = Url.Action("ObterEndereco", "Eventos", new { id = eventoViewModel.Id});
        return Json(new { success = true, data = url});
      }

      return PartialView("_IncluirEndereco", eventoViewModel);
    }

    public IActionResult AtualizarEndereco(Guid? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var eventoViewModel = _eventoServices.ObterPorId(id.Value);
      return PartialView("_IncluirEndereco", eventoViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AtualizarEndereco(EventoViewModel eventoViewModel)
    {
      ModelState.Clear();
      eventoViewModel.Endereco.EventoId = eventoViewModel.Id;
      _eventoServices.AtualizarEndereco(eventoViewModel.Endereco);

      if (OperacaoValida())
      {
        string url = Url.Action("ObterEndereco", "Eventos", new { id = eventoViewModel.Id });
        return Json(new { success = true, data = url });
      }

      return PartialView("_AtualizarEndereco", eventoViewModel);
    }


    public IActionResult ObterEndereco(Guid id)
    {
      return PartialView("_DetalhesEndereco");
    }

  }
}

﻿using Core.Domain.CommandHandlers;
using Core.Domain.Core.Bus;
using Core.Domain.Core.Events;
using Core.Domain.Core.Notifications;
using Core.Domain.Eventos.Events;
using Core.Domain.Models.Eventos;
using Core.Domain.Eventos.Repository;
using Core.Domain.Interfaces;
using System;

namespace Core.Domain.Eventos.Commands
{
  public class EventoCommandHandler : CommandHandler,
    IHandler<RegistrarEventoCommand>,
    IHandler<AtualizarEventoCommand>,
    IHandler<ExcluirEventoCommand>
  {

    private readonly IEventoRepository _eventoRepository;
    private readonly IBus _bus;
    private readonly IDomainNotificationHandler<DomainNotification> _notifications;

    public EventoCommandHandler(
      IEventoRepository eventoRepository,
      IUnitOfWork uow,
      IBus bus,
      IDomainNotificationHandler<DomainNotification> notifications)
      : base(uow, bus, notifications)
    {
      _eventoRepository = eventoRepository;
      _bus = bus;
      _notifications = notifications;
    }

    public void Handle(RegistrarEventoCommand message)
    {
      var endereco = new Endereco(message.Endereco.Id, message.Endereco.Logradouro, message.Endereco.Numero, message.Endereco.Bairro, message.Endereco.CEP, message.Endereco.Complemento, message.Endereco.Cidade, message.Endereco.Estado, message.Endereco.EventoId.Value);
      var evento = Evento.EventoFactory.NovoEventoCompleto(
          message.Id, message.Nome, message.Descricao,
          message.DescricaoCurta, message.DataInicio, message.DataFim, message.Gratuito,
          message.Valor, message.Online, message.NomeEmpresa, message.OrganizadorId, endereco, message.CategoriaId);

      if (!EventoValido(evento)) return;

      // TODO:
      // Validações de negócio
      // Organizador por registrar evento?

      _eventoRepository.Adicionar(evento);

      if (Commit())
        _bus.RaiseEvent(new EventoRegistradoEvent(evento.Id, evento.Nome, evento.DataInicio, evento.DataFim, evento.Gratuito, evento.Valor, evento.Online, evento.NomeEmpresa));
    }

    public void Handle(ExcluirEventoCommand message)
    {
      if (!EventoExiste(message.Id, message.MessageType)) return;

      _eventoRepository.Remover(message.Id);
      if (Commit())
        _bus.RaiseEvent(new EventoExcluidoEvent(message.Id));
    }

    public void Handle(AtualizarEventoCommand message)
    {
      if (!EventoExiste(message.Id, message.MessageType)) return;

      var eventoAtual = _eventoRepository.ObterPorId(message.Id);

      // Quem edita é o dono do evento?

      var evento = Evento.EventoFactory.NovoEventoCompleto(
          message.Id, message.Nome, message.Descricao,
          message.DescricaoCurta, message.DataInicio, message.DataFim, message.Gratuito,
          message.Valor, message.Online, message.NomeEmpresa, message.OrganizadorId, eventoAtual.Endereco, message.CategoriaId);

      if (!EventoValido(evento)) return;

      _eventoRepository.Atualizar(evento);
      if (Commit())
      {
        _bus.RaiseEvent(new EventoAtualizadoEvent(
          evento.Id, evento.Nome, evento.Descricao, evento.DescricaoCurta,
          evento.DataInicio, evento.DataFim, evento.Gratuito, evento.Valor,
          evento.Online, evento.NomeEmpresa));
      }
    }

    private bool EventoValido(Evento evento)
    {
      if (evento.EhValido()) return true;

      NotificarValidacoesErro(evento.ValidationResult);
      return false;
    }

    private bool EventoExiste(Guid id, string messageType)
    {
      var evento = _eventoRepository.ObterPorId(id);
      if (evento != null) return true;

      _bus.RaiseEvent(new DomainNotification(messageType, "Evento não encontrado."));
      return false;
    }
  }
}

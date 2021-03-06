﻿using AutoMapper;
using Core.Application.Interfaces;
using Core.Application.Services;
using Core.Domain.Core.Bus;
using Core.Domain.Core.Events;
using Core.Domain.Core.Notifications;
using Core.Domain.Eventos.Commands;
using Core.Domain.Eventos.Events;
using Core.Infra.Bus;
using Core.Infra.Data.Context;
using Core.Infra.Data.Repository;
using Core.Infra.Data.UoW;
using Core.Domain.Eventos.Repository;
using Core.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Core.Domain.Organizadores.Commands;
using Core.Domain.Organizadores.Events;
using Core.Domain.Organizadores.Repository;
using Microsoft.AspNetCore.Http;
using Core.Infra.Identity.Services;
using Core.Infra.Identity.Models;

namespace Core.Infra.IoC
{
  public class NativeInjectorBootstraper
  {
    public static void RegisterServices(IServiceCollection services)
    {
      // Application
      services.AddSingleton(Mapper.Configuration);
      services.AddScoped<IMapper>(s => new Mapper(s.GetRequiredService<IConfigurationProvider>(), s.GetServices));
      services.AddScoped<IEventoServices, EventoServices>();
      services.AddScoped<IOrganizadorServices, OrganizadorServices>();

      // Domain - Commands
      services.AddScoped<IHandler<RegistrarEventoCommand>, EventoCommandHandler>();
      services.AddScoped<IHandler<AtualizarEventoCommand>, EventoCommandHandler>();
      services.AddScoped<IHandler<ExcluirEventoCommand>, EventoCommandHandler>();
      services.AddScoped<IHandler<AtualizarEnderecoCommand>, EventoCommandHandler>();
      services.AddScoped<IHandler<IncluirEnderecoCommand>, EventoCommandHandler>();
      services.AddScoped<IHandler<RegistrarOrganizadorCommand>, OrganizadorCommandHandler>();

      // Domain - Events
      services.AddScoped<IHandler<EventoRegistradoEvent>, EventoEventHandler>();
      services.AddScoped<IHandler<EventoAtualizadoEvent>, EventoEventHandler>();
      services.AddScoped<IHandler<EventoExcluidoEvent>, EventoEventHandler>();
      services.AddScoped<IHandler<EnderecoAdicionadoEvent>, EventoEventHandler>();
      services.AddScoped<IHandler<EnderecoAtualizadoEvent>, EventoEventHandler>();
      services.AddScoped<IHandler<OrganizadorRegistradoEvent>, OrganizadorEventHandler>();

      // Domain - Notifications
      services.AddScoped<IDomainNotificationHandler<DomainNotification>, DomainNotificationHandler>();

      // Infra - Repositories
      services.AddScoped<IEventoRepository, EventoRepository>();
      services.AddScoped<IOrganizadorRepository, OrganizadorRepository>();

      // Infra - Data
      services.AddScoped<IUnitOfWork, UnitOfWork>();
      services.AddScoped<CoreContext>();
      services.AddScoped<IBus, InMemoryBus>();

      // Infra - Identity
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
      services.AddTransient<IEmailSender, AuthMessageSender>();
      services.AddTransient<ISmsSender, AuthMessageSender>();
      services.AddScoped<IUser, AspNetUser>();
    }
  }
}

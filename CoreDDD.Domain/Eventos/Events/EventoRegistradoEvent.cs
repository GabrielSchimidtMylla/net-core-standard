﻿using Core.Domain.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Eventos.Events
{
  public class EventoRegistradoEvent : BaseEventoEvent
  {
    public EventoRegistradoEvent(Guid id, string pNome, DateTime pDataInicio, DateTime pDataFim,
        bool pGratuito, decimal pValor, bool pOnline, string pNomeEmpresa)
    {
      Id = id;
      Nome = pNome;
      DataInicio = pDataInicio;
      Valor = pValor;
      Gratuito = pGratuito;
      Online = pOnline;
      NomeEmpresa = pNomeEmpresa;

      AggregateId = id;
    }
  }
}

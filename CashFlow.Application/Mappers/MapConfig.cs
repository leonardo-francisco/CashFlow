using AutoMapper;
using CashFlow.Application.DTOs;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Enums;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Application.Mappers
{
    public class MapConfig : Profile
    {
        private const string BrDateFormat = "dd-MM-yyyy";
        public MapConfig()
        {
            // TransactionDto -> Transaction
            CreateMap<TransactionDto, Transaction>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(d => d.Type, o => o.MapFrom(src =>
                    src.Type == "Crédito" ? TransactionType.Credit : TransactionType.Debit))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date));


            // Transaction -> TransactionDto
            CreateMap<Transaction, TransactionDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src =>
                    src.Type == TransactionType.Credit ? "Crédito" : "Débito"))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date));


            // CreateTransactionDto -> TransactionDto
            CreateMap<CreateTransactionDto, TransactionDto>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src =>
                    src.Type == TransactionType.Credit ? "Crédito" : "Débito"))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src =>
                    DateOnly.ParseExact(src.Date, BrDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None)));
        }
    }
}

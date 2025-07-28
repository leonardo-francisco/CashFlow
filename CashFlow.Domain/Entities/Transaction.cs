using CashFlow.Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Domain.Entities
{
    public sealed record class Transaction : BaseEntity
    {
        [BsonElement("userId")]
        public required string UserId { get; init; }

        [BsonElement("amount")]
        public required decimal Amount { get; init; }

        [BsonElement("type")]
        public required TransactionType Type { get; init; }

        [BsonElement("date")]
        public required DateOnly Date { get; init; }

        [BsonElement("description")]
        public string? Description { get; init; }
    }
}

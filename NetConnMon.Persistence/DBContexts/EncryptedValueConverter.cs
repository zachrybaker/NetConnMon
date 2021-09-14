using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NetConnMon.Domain.Logic;

namespace NetConnMon.Persistence.DBContexts
{
	public interface IEncryptedValueConverter { }

	public class EncryptedValueConverter : ValueConverter<string, string>, IEncryptedValueConverter
	{
		IEncryptor encryptor;
		public EncryptedValueConverter(IEncryptor encryptor, ConverterMappingHints mappingHints = default)
			: base(s => encryptor.Encrypt(s), s => encryptor.Decrypt(s), mappingHints)
		{ this.encryptor = encryptor; }
	}
}

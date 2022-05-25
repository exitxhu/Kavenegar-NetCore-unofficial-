﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kavenegar_NetCore_unofficial_
{
    public enum MessageType
    {
        Flash = 0,
        MobileMemory = 1,
        SimMemory = 2,
        AppMemory = 3
    }
	public record SendResult(long Messageid ,int Cost , long Date, string Message, string Receptor, string Sender, int Status ,string StatusText) { 
		public DateTime GregorianDate
		{
			get { return Date.UnixTimestampToDateTime(); }
		}
	}
    public record ReturnSend(Result @Return ,List<SendResult> entries );

    public record Result(int status ,string message );
    
}

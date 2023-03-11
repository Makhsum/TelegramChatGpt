using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPT.Services.abstractions
{
    public interface ITelegramBotService
    {
        Task StartWorkingAsync();
    }
}

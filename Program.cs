using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Exceptions;
using System.Threading;

namespace tgbot
{

    internal class Program
    {
        static Tutor tutor = new Tutor();
        static WordStorage storage = new WordStorage();
        static TelegramBotClient Bot = new TelegramBotClient("TGTOKEN");
        static Dictionary<long, string> lastWord = new Dictionary<long, string>();
        const string COMMAND_LIST =
            @"Список команд: 
            /add <eng><rus> - добавление английского слова и
             его перевод в словарь.
            /get - получение случайного слова.";
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }
            };

            Bot.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken: cts.Token);

            Console.ReadLine();
           
        }

        private static Task HandleErrorAsync(ITelegramBotClient Bot, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegra API ERROR: \n " +
                $"{apiRequestException.ErrorCode}\n" +
                $" {apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }


        static string AddWord(String[] msgArr)
        {
            if (msgArr.Length != 3)
            {
                return "Неправильное колличество аргументов. их должно быть 2";
            }
            else
            {
                tutor.AdWord(msgArr[1].ToLower(), msgArr[2].ToLower());
                return "новое слово добавленно в словарь";
            }
        }

        private static string GetRandomWord(long chatID)
        {
            var text = tutor.GetRandomWord();
            if (lastWord.ContainsKey(chatID))
            {
                lastWord[chatID] = text;
            }
            else 
                lastWord.Add(chatID, text);
            return text;
        }


        private static async Task HandleUpdateAsync(ITelegramBotClient Bot, Update update, CancellationToken cancellationToken)
        {
            if(update.Type == Telegram.Bot.Types.Enums.UpdateType.Message &&  update.Message!.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                var chatId = update.Message.Chat.Id;
                var messageText = update.Message.Text;
                var firstName = update.Message.From.FirstName;
                var msgArgs = messageText.Split(' ');
                Message sendMessage;
                String text;
                switch (msgArgs[0])
                {
                    case "/start":
                        text = COMMAND_LIST;
                        break;
                    case "/add":
                        text = AddWord(msgArgs);
                        break;
                    case "/get":
                        text = GetRandomWord(chatId);
                        break;
                    default:
                        if (lastWord.ContainsKey((chatId)))
                        {

                            text = CheckWord(lastWord[chatId], msgArgs[0]);
                            var newWord = GetRandomWord((chatId));
                            text = $"{text}\r\nСледующее слово {newWord}";
                        }
                        else
                        {
                            text = COMMAND_LIST;
                        }
                        break;
                }
               sendMessage = await Bot.SendTextMessageAsync(chatId, text, cancellationToken: cancellationToken);
               
            }
        }

        private static string CheckWord(string eng, string rus)
        {
            if (tutor.CheckWord(eng.ToLower(), rus.ToLower()))               
                return "Правильно!";                
            else
            {
                var correctAnswer = tutor.Translate(eng.ToLower());
                return $"Неверно! Правильный ответ {correctAnswer}";
            }
        }
    }
}

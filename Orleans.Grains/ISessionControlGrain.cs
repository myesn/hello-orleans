using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orleans.Grains
{
    public interface ISessionControlGrain : IGrainWithStringKey
    {
        Task Login(string userId);
        Task Logout(string userId);
        Task<int> GetActiveUserCount();
    }

    public class SessionControlGranin : Grain, ISessionControlGrain
    {
        private List<string> _loginUsers { get; set; } = new List<string>();

        public Task Login(string userId)
        {
            var appName = this.GetPrimaryKeyString();

            _loginUsers.Add(userId);

            Console.WriteLine($"Current active users count of {appName} is {_loginUsers.Count}");
            return Task.CompletedTask;
        }

        public Task Logout(string userId)
        {
            //获取当前Grain的身份标识
            var appName = this.GetPrimaryKey();
            _loginUsers.Remove(userId);

            Console.WriteLine($"Current active users count of {appName} is {_loginUsers.Count}");
            return Task.CompletedTask;
        }

        public Task<int> GetActiveUserCount()
        {
            return Task.FromResult(_loginUsers.Count);
        }
    }
}

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace EBlazorExample.Data
{
    public class UserService
    {
        private readonly NavigationManager hostEnvironment;

        public UserService(IJSRuntime js,NavigationManager hostEnvironment)
        {
            Js = js;
            this.hostEnvironment = hostEnvironment;
        }
        private string BaseUrl => "http://hrapi.rwaafid.com/HrEmployee";

        //[Inject]
        private HttpClient Http { get; set; } = new HttpClient();
        public IJSRuntime Js { get; }

        public async Task<IEnumerable<User>> GetUsers(int pageIndex,int pageSize)
        {

            //Skip((pageIndex - 1) * 10).Take(10)
            int skip = (pageIndex - 1) * pageSize;
            string url = $"{BaseUrl}?$skip={skip}&$top={pageSize}&$select=EmpID,EmpName,UserNumber,CityName,BirthDate,Mobile";
            var response = await Http.GetAsync(url);
            var root = Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(await response.Content.ReadAsStringAsync());
            return root.value;
        }

        public async Task<IEnumerable<User>> FinByNameAsync(string name)
        {
            string url = $"{BaseUrl}?$filter=contains(EmpName,'{name}')&$select=EmpID,EmpName,UserNumber,CityName,BirthDate,Mobile";
            await Js.InvokeVoidAsync("console.log", url);
            var response = await Http.GetAsync(url);
            var root = Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(await response.Content.ReadAsStringAsync());
            return root.value;
        }

        public async Task<IEnumerable<User>> MockDataAsync(Func<User, bool> predicate = null)
        {
            HttpClient http = new HttpClient();
            http.BaseAddress = new Uri(hostEnvironment.BaseUri);
            string url = "MOCK_DATA.json";
            var response = await http.GetFromJsonAsync<IEnumerable<User>>(url);
            //var root = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<User>>(await response.Content.ReadAsStringAsync());
            //return root;
            if(predicate == null)
            {
                return response;
            }
            return response.Where(predicate);
        }
    }

    public class User
    {
        public int EmpID { get; set; }
        public string EmpName { get; set; }
        public string UserNumber { get; set; }
        public string CityName { get; set; }
        public string Mobile { get; set; }
        public string BirthDate { get; set; }
    }

    

    public class Root
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        public List<User> value { get; set; }
    }
}

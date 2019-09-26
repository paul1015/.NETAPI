using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using atm.Models;
using Nest;
using System.Text;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

//atm/api

namespace atm.Controllers
{
    //Set api as Controller name
    [Route("api/[controller]")]
    [ApiController]
    public class AtmController : ControllerBase
    {
        private readonly AtmContext _context;

        //Initilize API unused
        public AtmController(AtmContext context)
        {
            _context = context;
            Console.Write("context = {0} ", _context);
            Console.WriteLine("AtmController");

            if (_context.AtmItems.Count() == 0)
            {
                Console.WriteLine("If context AtmController");
                _context.AtmItems.Add(new AtmItem { type = "Item1" });
                _context.SaveChanges();
            }

        }

        //Http Get All Data api/atm

        [HttpGet]
        public IReadOnlyCollection<AtmItem> GetAllitem()
        { 
            Console.WriteLine("Get AtmController");
            //Connect to ElasticSearch

            var settings = new ConnectionSettings(new Uri("http://elastic:changeme@140.115.54.75:9200")).DefaultIndex("flowdata924");
            var client = new ElasticClient(settings);

            //Get Randon 50 data in ElasticSearch index

            var searchResponse = client.Search<AtmItem>(s => s
                   .Size(50)
                   .Query(q => q.MatchAll()
                )
            );

            var documents = searchResponse.Documents.ToList();

            //Return Data
            return documents;
        }

        // Http Get specific ID Data

        [HttpGet("{id}")]
        public IReadOnlyCollection<AtmItem> GeAtmItem(int id)
        {
            Console.WriteLine("Get From ID");

            //Get indicated data in ElasticSearch index

            var settings = new ConnectionSettings(new Uri("http://elastic:changeme@140.115.54.75:9200")).DefaultIndex("flowdata924");
            var client = new ElasticClient(settings);
            string stringID = id.ToString();

            var searchResponse = client.Search<AtmItem>(s => s
                .Query(q => q
                    .Match(m => m
                    .Field(f => f.id)
                    .Query(stringID)
                )
             )
            );

            var reponseAtmItem = searchResponse.Documents;

            //Return indicated data

            return reponseAtmItem;

        }

        // Receive Post data api/atm method: Post

        [HttpPost]
        public AtmItem Post()
        { 
            Console.WriteLine("PostController");
            Debug.WriteLine("PostController");


            
            string documentContents;

            Debug.WriteLine("ContentType =  : {0}", Request.Headers);

            // Receive Data 
            using (Stream receiveStream = Request.Body)
            {
                
                using (StreamReader readStream = new StreamReader(receiveStream,Encoding.UTF8))
                {
                    documentContents = readStream.ReadToEnd();
                }

            }

            //Deserilize the JSON Data

            List<AtmItem> result = JsonConvert.DeserializeObject<List<AtmItem>>(documentContents);

            int c = result.Count;

            for (int i = 0; i < c; i++)
            {
                Console.WriteLine("Value of i: {0}", i);
                Debug.WriteLine("Value of i: {0}", i);

                // Put Data in ElasticSearch and give it ID

                _context.AtmItems.Add(result[i]);
                result[i].id = i;
                ElesticPost(result[i]);

            }

            //Ignore it
            return result[1];

        }

        //Update the data in ElasticSearch api/atm Method:Put
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAtmItem(int id)
        {
            Console.WriteLine("Put AtmController");

            //Call ElasticUpdate function
            ElasticPut(id);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Delete Data in ElasticSearch api/atm Method:Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAtmItem(int id)
        {
            Console.WriteLine("Delete");

            ElasticDelete(id);
            await _context.SaveChangesAsync();

            return NoContent();

        }

        //Post Function
        public void ElesticPost(AtmItem item)
        {

            Debug.WriteLine("ElasticPost");
            Console.WriteLine("ElasticPost");

            var settings = new ConnectionSettings(new Uri("http://elastic:changeme@140.115.54.75:9200")).DefaultIndex("flowdata924");
            var client = new ElasticClient(settings);

            var indexResponse = client.IndexDocument(item);

            Console.Write("x= {0} ", indexResponse);

        }

        //Delete Finction
        public void ElasticDelete(int id)
        {

            Debug.WriteLine("ElasticDelete");
            Console.WriteLine("ElasticDelete");

            var settings = new ConnectionSettings(new Uri("http://elastic:changeme@140.115.54.75:9200"));
            var client = new ElasticClient(settings);
            var deleteResponse = client.Delete(new DeleteRequest("flowdata924", id));

        }

        //Update Funtion
        public void ElasticPut(int id)
        {
            var settings = new ConnectionSettings(new Uri("http://elastic:changeme@140.115.54.75:9200")).DefaultIndex("flowdata924");
            var client = new ElasticClient(settings);
            string stringID = id.ToString();
            client.UpdateByQuery<AtmItem>(u => u
                .Query(q => q
                .Term(f => f.id, stringID)
                 )
                .Script("ctx._source.x = '88825252'")
                .Refresh(true)
             );
        }

    }
}

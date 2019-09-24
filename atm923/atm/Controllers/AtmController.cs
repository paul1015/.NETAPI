using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using atm.Models;
using Nest;

using System.Runtime.Serialization.Json;
using System.Text;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using System.Data;


namespace atm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AtmController : ControllerBase
    {
        private readonly AtmContext _context;

        public AtmController(AtmContext context)
        {
            _context = context;
            Console.Write("context = {0} ", _context);
            Console.WriteLine("AtmController");

            /*if (_context.AtmItems.Count() == 0)
            {
                Console.WriteLine("If context AtmController");
                // Create a new TodoItem if collection is empty,
               // which means you can't delete all TodoItems.
                _context.AtmItems.Add(new AtmItem { type = "Item1" });
                _context.SaveChanges();
            }*/

        }

        // GET: api/Todo/5
        // GET: api/Todo

        //Ignore it, I'm still not find out how to use it.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AtmItem>>> GetAtmItems()
        {
            Console.WriteLine("Get AtmController");

            return await _context.AtmItems.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AtmItem>> GetAtmItem(int id)
        {

            Console.WriteLine("Get ID AtmController");

            var AtmItem = await _context.AtmItems.FindAsync(id);

            if (AtmItem == null)
            {
                return NotFound();
            }

            return AtmItem;
        }

        //Main Post function
        [HttpPost]
        public async Task<ActionResult> Post()
        {
            Console.WriteLine("PostController");
            Debug.WriteLine("PostController");


           //Use documentContects to store the input JSON string
           string documentContents;

           Debug.WriteLine("ContentType =  : {0}", Request.Headers);

            //Receive Json string post method in receiveStream
            using (Stream receiveStream = Request.Body)
            { 
                //Encoding the data into documentContent
                using (StreamReader readStream = new StreamReader(receiveStream,Encoding.UTF8))
                {
                    documentContents = readStream.ReadToEnd();
                }

            }

            // Deserilization the JSON string into Json file accordin to AtmItem module
            List<AtmItem> result = JsonConvert.DeserializeObject<List<AtmItem>>(documentContents);

            int c = result.Count;

            //Put each ATM foemat into ElasticFunction
            for (int i = 0; i < c; i++)
            {
                Console.WriteLine("Value of i: {0}", i);
                Debug.WriteLine("Value of i: {0}", i);
                ElesticPost(result[i]);
            }

            await _context.SaveChangesAsync();

            //return CreatedAtAction(nameof(GetAtmItem), new { id ='1' },"");

            return CreatedAtAction(nameof(GetAtmItem), new { id = '1' }, "");

        }

      

        public void ElesticPost(AtmItem item)
        {
            Debug.WriteLine("ElasticPost");
            Console.WriteLine("ElasticPost");

            //Connect with Elastiosearch
            var settings = new ConnectionSettings(new Uri("http://elastic:changeme@140.115.54.69:9200")).DefaultIndex("file539");
            var client = new ElasticClient(settings);

            // Put data in Elastic search;
            var indexResponse = client.IndexDocument(item);

            Console.Write("x= {0} ", indexResponse);

        }
    }
}

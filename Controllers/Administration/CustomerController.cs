using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web;
using System.Collections.Specialized;

using CabernetDBContext;
using FoulNet.Model;
using FoulNet;

namespace FoulNet
{
    [Route("api/Customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly Entities db;
        public CustomerController(Entities context)
        {
            db = context;
        }

        // GET: api/Customer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomer()
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(this.Request.QueryString.Value);
            var languageID = this.CurrentLanguageID();
            var previewList = new[] { ".jpeg", ".jpg", ".png", ".gif", ".bmp" };
            var list = (
                from Customer in db.Customer
                join State in db.State on Customer.StateID equals State.StateID
                select new
                {
                    Customer.CustomerID,
                    Customer.CustomerName,
                    Customer.StateID,
                    State.StateName,
                }

            );
            if (parameters["key"] != null)
            {
                string key = parameters["key"];
                list = list.Where(l => l.CustomerID == key);
            }

            if (parameters["CustomerID"] != null)
            {
                string customerID = parameters["CustomerID"];
                list = list.Where(l => l.CustomerID == customerID);
            }


            if (parameters["CustomerName"] != null)
            {
                string customerName = parameters["CustomerName"];
                list = list.Where(l => l.CustomerName.Contains(customerName));
                list = list.OrderBy(l => l.CustomerName.IndexOf(customerName));
            }

            if (parameters["StateID"] != null)
            {
                string stateID = parameters["StateID"];
                list = list.Where(l => l.StateID == stateID);
            }

            var ret = await list.ToListAsync();
            return Ok(ret);


        }

        // GET: api/Customer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(string id)
        {
            if (db.Customer == null)
            {
                return NotFound();
            }
            var Customer = await db.Customer.FindAsync(id);

            if (Customer == null)
            {
                return NotFound();
            }

            return Customer;
        }

        // PUT: api/Customer/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(string id, Customer Customer)
        {
            if (id != Customer.CustomerID)
            {
                return BadRequest();
            }

            db.Entry(Customer).State = EntityState.Modified;
            Entities.ProcessChildrenUpdate(db, Customer.DataTranslation.ToList());

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Customer
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer Customer)
        {
            if (db.Customer == null)
            {
                return Problem("Entity set 'Entities.Customer'  is null.");
            }
            db.Customer.Add(Customer);
            Entities.ProcessChildrenUpdate(db, Customer.DataTranslation.ToList());
            await db.SaveChangesAsync();

            return CreatedAtAction("GetCustomer", new { id = Customer.CustomerID }, Customer);
        }

        // DELETE: api/Customer/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Customer>> DeleteCustomer(string id)
        {
            if (db.Customer == null)
            {
                return NotFound();
            }
            var Customer = await db.Customer.FindAsync(id);
            if (Customer == null)
            {
                return NotFound();
            }

            db.Customer.Remove(Customer);
            await db.SaveChangesAsync();

            return Customer;
        }

        private bool CustomerExists(string id)
        {
            return (db.Customer?.Any(e => e.CustomerID == id)).GetValueOrDefault();
        }
    }
}
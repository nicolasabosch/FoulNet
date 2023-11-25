using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoulNet;
using FoulNet.Model;
using System.Collections.Specialized;
using System.Web;
using CabernetDBContext;

namespace FoulNet.Controllers.Administration
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly Entities _context;

        public VendorController(Entities context)
        {
            _context = context;
        }

        // GET: api/Vendor
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vendor>>> GetVendor()
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(this.Request.QueryString.Value);
            var languageID = this.CurrentLanguageID();
            var previewList = new[] { ".jpeg", ".jpg", ".png", ".gif", ".bmp" };
            var list = (
                from Vendor in _context.Vendor
                select new
                {
                    Vendor.VendorID,
                    Vendor.VendorName,
                }

            );
            if (parameters["key"] != null)
            {
                int key = int.Parse(parameters["Key"]);
                list = list.Where(l => l.VendorID == key);
            }

            if (parameters["VendorID"] != null)
            {
                int vendorID = int.Parse( parameters["VendorID"]);
                list = list.Where(l => l.VendorID == vendorID);
            }

            if (parameters["VendorName"] != null)
            {
                string vendorName = parameters["VendorName"];
                list = list.Where(l => l.VendorName.Contains(vendorName));
                list = list.OrderBy(l => l.VendorName.IndexOf(vendorName));
            }

            var ret = await list.ToListAsync();
            return Ok(ret);
        }

        // GET: api/Vendor/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Vendor>> GetVendor(int id)
        {
            if (_context.Vendor == null)
            {
                return NotFound();
            }
            var Vendor = await _context.Vendor.FindAsync(id);

            if (Vendor == null)
            {
                return NotFound();
            }

            return Vendor;
        }

        // PUT: api/Vendor/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVendor(int id, Vendor vendor)
        {
            if (id != vendor.VendorID)
            {
                return BadRequest();
            }

            _context.Entry(vendor).State = EntityState.Modified;
            Entities.ProcessChildrenUpdate(_context, vendor.DataTranslation.ToList());

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VendorExists(id))
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

        // POST: api/Vendor
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Vendor>> PostVendor(Vendor vendor)
        {
            if (_context.Vendor == null)
            {
                return Problem("Entity set 'Entities.Vendor'  is null.");
            }
            _context.Vendor.Add(vendor);
            Entities.ProcessChildrenUpdate(_context, vendor.DataTranslation.ToList());
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVendor", new { id = vendor.VendorID }, vendor);
        }

        // DELETE: api/Vendor/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Vendor>> DeleteVendor(int id)
        {
            if (_context.Vendor == null)
            {
                return NotFound();
            }
            var Vendor = await _context.Vendor.FindAsync(id);
            if (Vendor == null)
            {
                return NotFound();
            }

            _context.Vendor.Remove(Vendor);
            await _context.SaveChangesAsync();

            return Vendor;
        }

        private bool VendorExists(int id)
        {
            return (_context.Vendor?.Any(e => e.VendorID == id)).GetValueOrDefault();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service1.Datas;

namespace Service1.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase {
        private ApplicationDbContext _context;

        public ProductsController (ApplicationDbContext context) {
            _context = context;
        }

        [HttpGet]
        public ActionResult Get () {
            return Ok (_context.Products.ToList ());
        }

        [HttpGet ("{id}")]
        public ActionResult Get (Guid id) {
            var result = _context.Products.SingleOrDefault (x => x.ProductId == id);
            if (result == null)
                return NotFound ();
            return Ok (result);
        }

        [HttpPost]
        public async Task<ActionResult> Post ([FromBody] Product product) {
            if (product.ProductId == Guid.Empty)
                product.ProductId = Guid.NewGuid ();
            await _context.Products.AddAsync (product);
            await _context.SaveChangesAsync ();
            return Ok ();
        }

        [HttpPut ("{id}")]
        public async Task<ActionResult> Put (Guid id, [FromBody] Product product) {
            if (product.ProductId != id)
                return BadRequest ();
            var result = _context.Products.SingleOrDefault (x => x.ProductId == id);
            if (result == null)
                return NotFound ();

            _context.Entry (result).CurrentValues.SetValues (product);
            await _context.SaveChangesAsync ();

            return Ok ();
        }

        // DELETE api/values/5
        [HttpDelete ("{id}")]
        public async Task<ActionResult> Delete (Guid id) {
            var result = _context.Products.SingleOrDefault (x => x.ProductId == id);
            if (result == null)
                return NotFound ();
            _context.Products.Remove(result);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
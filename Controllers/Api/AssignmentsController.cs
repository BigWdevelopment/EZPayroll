using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EZPayroll.MVC.Models;

namespace EZPayroll.MVC.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentsController : ControllerBase
    {
        private readonly SystemDbContext _context;

        public AssignmentsController(SystemDbContext context)
        {
            _context = context;
        }

        // GET: api/Assignments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Assignments>>> GetAssignments()
        {
            return await _context.Assignments.ToListAsync();
        }

        // GET: api/Assignments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Assignments>> GetAssignments(int id)
        {
            var assignments = await _context.Assignments.FindAsync(id);

            if (assignments == null)
            {
                return NotFound();
            }

            return assignments;
        }

        // PUT: api/Assignments/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAssignments(int id, Assignments assignments)
        {
            if (id != assignments.AssignmentId)
            {
                return BadRequest();
            }

            _context.Entry(assignments).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssignmentsExists(id))
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

        // POST: api/Assignments
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Assignments>> PostAssignments(Assignments assignments)
        {
            _context.Assignments.Add(assignments);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAssignments", new { id = assignments.AssignmentId }, assignments);
        }

        // DELETE: api/Assignments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Assignments>> Delete(int id)
        {
            var assignments = await _context.Assignments.FindAsync(id);            

            if (assignments == null)
            {
                return NotFound();
            }
            var hours = await _context.EmployeeHours.Where(d => d.TaskId == assignments.TaskId && d.EmpId == assignments.EmpId).ToListAsync();
            if (hours.Count > 0)
            {
                return NotFound();
            }

            _context.Assignments.Remove(assignments);
            await _context.SaveChangesAsync();
            return assignments;
        }

        private bool AssignmentsExists(int id)
        {
            return _context.Assignments.Any(e => e.AssignmentId == id);
        }
    }
}

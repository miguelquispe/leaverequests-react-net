using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LeaveRequestAPI.Data;
using LeaveRequestAPI.Models;

namespace LeaveRequestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LeaveRequestsController> _logger;

        public LeaveRequestsController(AppDbContext context, ILogger<LeaveRequestsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/LeaveRequests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveRequest>>> GetLeaveRequest()
        {
            _logger.LogInformation("GET all leave requests");
            return await _context.LeaveRequest.ToListAsync();
        }

        // GET: api/LeaveRequests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveRequest>> GetLeaveRequest(int id)
        {
            _logger.LogInformation("GET leave request {LeaveRequestId}", id);

            var leaveRequest = await _context.LeaveRequest.FindAsync(id);

            if (leaveRequest == null)
            {
                _logger.LogWarning("Leave request {LeaveRequestId} not found", id);
                return NotFound();
            }

            return leaveRequest;
        }

        // PUT: api/LeaveRequests/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLeaveRequest(int id, LeaveRequest leaveRequest)
        {
            _logger.LogInformation("PUT leave request {LeaveRequestId}", id);

            if (id != leaveRequest.Id)
            {
                _logger.LogWarning("PUT leave request id mismatch: {LeaveRequestId}", id);
                return BadRequest();
            }

            _context.Entry(leaveRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeaveRequestExists(id))
                {
                    _logger.LogWarning("PUT leave request {LeaveRequestId} not found", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError("PUT concurrency error for leave request {LeaveRequestId}", id);
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/LeaveRequests
        [HttpPost]
        public async Task<ActionResult<LeaveRequest>> PostLeaveRequest(LeaveRequest leaveRequest)
        {
            _logger.LogInformation("POST new leave request");

            _context.LeaveRequest.Add(leaveRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLeaveRequest", new { id = leaveRequest.Id }, leaveRequest);
        }

        // DELETE: api/LeaveRequests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeaveRequest(int id)
        {
            _logger.LogInformation("DELETE leave request {LeaveRequestId}", id);

            var leaveRequest = await _context.LeaveRequest.FindAsync(id);
            if (leaveRequest == null)
            {
                _logger.LogWarning("DELETE leave request {LeaveRequestId} not found", id);
                return NotFound();
            }

            _context.LeaveRequest.Remove(leaveRequest);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LeaveRequestExists(int id)
        {
            return _context.LeaveRequest.Any(e => e.Id == id);
        }
    }
}

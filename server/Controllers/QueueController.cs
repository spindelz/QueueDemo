using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using server.Models;
using server.Repositories.Interface;
using System.Net;

namespace server.Controllers
{
    public class QueueController : ControllerBase
    {
        private readonly IQueue _queue;

        public QueueController(IQueue queue)
        {
            _queue = queue;
        }

        [HttpPost("[controller]/receive")]
        public async Task<IActionResult> ReceiveQueue()
        {
            try
            {
                Queue? lastestQueue = await _queue.GetLastestQueue();
                if (lastestQueue != null)
                {
                    if (lastestQueue.Number == "Z9")
                    {
                        return BadRequest(new
                        {
                            statusCode = (int)HttpStatusCode.BadRequest,
                            message = "Full Queue"
                        });
                    }
                }

                string q = await _queue.GenerateQueue();
                return Ok(new
                {
                    queueNumber = q
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new
                {
                    statusCode = (int)HttpStatusCode.InternalServerError,
                    message = ex.Message,
                    messageDetail = ex.ToString()
                });
            }
        }

        [HttpGet("[controller]/detail")]
        public async Task<IActionResult> GetQueueDetail(string qNum)
        {
            try
            {
                QueueResponse? q = await _queue.GetQueueDetail(qNum);
                if (q == null)
                {
                    return BadRequest(new
                    {
                        statusCode = (int)HttpStatusCode.BadRequest,
                        message = "Queue not found"
                    });
                }
                return Ok(q);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new
                {
                    statusCode = (int)HttpStatusCode.InternalServerError,
                    message = ex.Message,
                    messageDetail = ex.ToString()
                });
            }
        }

        [HttpPost("[controller]/clear")]
        public async Task<IActionResult> ClearQueue()
        {
            try
            {
                await _queue.ClearQueue();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new
                {
                    statusCode = (int)HttpStatusCode.InternalServerError,
                    message = ex.Message,
                    messageDetail = ex.ToString()
                });
            }
        }
    }
}

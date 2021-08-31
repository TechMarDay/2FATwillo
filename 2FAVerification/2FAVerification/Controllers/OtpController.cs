using _2FAVerification.Models;
using _2FAVerification.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace _2FAVerification.Controllers
{
    public class OtpController : Controller
    {
        private readonly IVerification _verification;

        public OtpController(IVerification verification)
        {
            _verification = verification;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("SendOtpCode")]
        public async Task<IActionResult> SendOtpCode()
        {
            await _verification.StartVerificationAsync("+84388653287", "sms");

            return RedirectToAction("SubmitOtpCode", "Otp", new { errorCode = 1 });
        }

        [HttpGet("SubmitOtpCode")]
        public IActionResult SubmitOtpCode([FromQuery] int errorCode)
        {
            if (errorCode != 1)
            {
                switch (errorCode)
                {
                    case 0:
                        ViewBag.ErrorMessage = "Otp fail";
                        return View();
                }
            }

            return View();
        }

        [HttpPost("VerifyOtpCode")]
        public async Task<IActionResult> VerifyOtpCodeAsync([FromForm] OtpModel model)
        {
            var code = Request.Form["Code"];

            var result = await _verification.CheckVerificationAsync("+84388653287", code);
            if (result.IsValid)
            {
                return RedirectToAction("Index", "Otp");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return RedirectToAction("SubmitOtpCode", "Otp", new { errorCode = 0 });
        }
    }
}
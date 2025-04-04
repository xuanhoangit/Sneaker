namespace SneakerAPI.Core.Libraries;
public static class EmailTemplateHtml
{
public static string RenderEmailNotificationBody(string username,string head,string content){
return  $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Notification</title>
</head>
<body style='margin: 0; padding: 0; background-color: #f4f4f4;'>
    <table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' bgcolor='#f4f4f4'>
        <tr>
            <td align='center'>
                <table role='presentation' cellspacing='0' cellpadding='0' border='0' width='600' style='background: #ffffff; margin: 20px auto; padding: 20px; border-radius: 8px;'>
                    <!-- Header -->
                    <tr>
                        <td align='center' style='padding: 20px 0;'>
                            <h1 style='margin: 0; font-family: Arial, sans-serif; color: #333;'>{head}</h1>
                        </td>
                    </tr>
                    <!-- Body -->
                    <tr>
                        <td style='padding: 20px; font-family: Arial, sans-serif; color: #555;'>
                            <p>Hello <strong>{username}</strong>,</p>
                            
                            <p>
                                {content}
                            </p>
                            <p>Regards</p>
                            <p><b>Sneaker Luxury Store</b></p>
                        </td>
                    </tr>
                    <!-- Footer -->
                    <tr>
                        <td align='center' style='padding: 20px; font-size: 14px; color: #777; font-family: Arial, sans-serif;'>
                            <p>&copy; 2025 Sneaker Luxury Store. All rights reserved.</p>
                            <p><a href='#' style='color: #007bff; text-decoration: none;'>Privacy policy</a> | <a href='#' style='color: #007bff; text-decoration: none;'>Terms and Services</a></p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
} 

public static string RenderEmailForgotPasswordBody(string username,string password){
return  $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Forgot Password</title>
</head>
<body style='margin: 0; padding: 0; background-color: #f4f4f4;'>
    <table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' bgcolor='#f4f4f4'>
        <tr>
            <td align='center'>
                <table role='presentation' cellspacing='0' cellpadding='0' border='0' width='600' style='background: #ffffff; margin: 20px auto; padding: 20px; border-radius: 8px;'>
                    <!-- Header -->
                    <tr>
                        <td align='center' style='padding: 20px 0;'>
                            <h1 style='margin: 0; font-family: Arial, sans-serif; color: #333;'>Welcome to you!</h1>
                        </td>
                    </tr>
                    <!-- Body -->
                    <tr>
                        <td style='padding: 20px; font-family: Arial, sans-serif; color: #555;'>
                            <p>Hello <strong>{username}</strong>,</p>
                            
                            <p style='text-align: center;'>
                                Your new password: <strong>{password}</strong>
                            </p>
                            <p>Regards</p>
                            <p><b>Sneaker Luxury Store</b></p>
                        </td>
                    </tr>
                    <!-- Footer -->
                    <tr>
                        <td align='center' style='padding: 20px; font-size: 14px; color: #777; font-family: Arial, sans-serif;'>
                            <p>&copy; 2025 Sneaker Luxury Store. All rights reserved.</p>
                            <p><a href='#' style='color: #007bff; text-decoration: none;'>Privacy policy</a> | <a href='#' style='color: #007bff; text-decoration: none;'>Terms and Services</a></p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
} 
public static string RenderEmailRegisterBody(string username,string otp){
return  $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Verify Email</title>
</head>
<body style='margin: 0; padding: 0; background-color: #f4f4f4;'>
    <table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' bgcolor='#f4f4f4'>
        <tr>
            <td align='center'>
                <table role='presentation' cellspacing='0' cellpadding='0' border='0' width='600' style='background: #ffffff; margin: 20px auto; padding: 20px; border-radius: 8px;'>
                    <!-- Header -->
                    <tr>
                        <td align='center' style='padding: 20px 0;'>
                            <h1 style='margin: 0; font-family: Arial, sans-serif; color: #333;'>Welcome to you!</h1>
                        </td>
                    </tr>
                    <!-- Body -->
                    <tr>
                        <td style='padding: 20px; font-family: Arial, sans-serif; color: #555;'>
                            <p>Hello <strong>{username}</strong>,</p>
                            <p>Thanks for your register. To complete the process please enter the otp code below: </p>
                            <p style='text-align: center;'>
                                Your OTP Code: <strong>{otp}</strong>
                            </p>
                            <p>This OTP Code will be expire after 5 minutes.</p>
                            <p>Regards</p>
                            <p><b>Sneaker Luxury Store</b></p>
                        </td>
                    </tr>
                    <!-- Footer -->
                    <tr>
                        <td align='center' style='padding: 20px; font-size: 14px; color: #777; font-family: Arial, sans-serif;'>
                            <p>&copy; 2025 Sneaker Luxury Store. All rights reserved.</p>
                            <p><a href='#' style='color: #007bff; text-decoration: none;'>Privacy policy</a> | <a href='#' style='color: #007bff; text-decoration: none;'>Terms and Services</a></p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
} 

}
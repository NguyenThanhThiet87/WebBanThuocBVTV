const apiKeySid = "SK.0.MuI0CozyHpKL4YT9EVEqkO7q7TioEicm";
const apiKeySecret = "cE9TdFpLVFBsMFRtTVZSQjNwSjlSTUprRVM4Zm1SeFQ=";

// SỐ MỚI BẠN ĐÃ MUA - ĐÂY LÀ SỐ "FROM" CHÍNH XÁC
const fromNumber = "Topica";

// THAY BẰNG SỐ NGƯỜI NHẬN - QUAN TRỌNG: PHẢI ĐÚNG ĐỊNH DẠNG E.164
const toNumber = "8468642533";   // Ví dụ: 84912345678

// --- CÁC HÀM BÊN DƯỚI ĐÃ ĐƯỢC ĐỊNH NGHĨA TRƯỚC KHI ĐƯỢC GỌI ---

function getAccessToken() {
    const now = Math.floor(Date.now() / 1000);
    const exp = now + 3600;
    const header = { cty: "stringee-api;v=1", alg: 'HS256', typ: 'JWT' };
    const payload = { jti: `${apiKeySid}-${now}`, iss: apiKeySid, exp: exp, rest_api: true };
    const sHeader = JSON.stringify(header);
    const sPayload = JSON.stringify(payload);
    return KJUR.jws.JWS.sign("HS256", sHeader, sPayload, { utf8: apiKeySecret });
}

async function sendSMS(smsArray) {
    const apiUrl = 'https://api.stringee.com/v1/sms';
    const accessToken = getAccessToken();
    const postData = JSON.stringify({ "sms": smsArray });

    try {
        const response = await fetch(apiUrl, {
            method: 'POST',
            headers: { 'X-STRINGEE-AUTH': accessToken, 'Content-Type': 'application/json', 'Accept': 'application/json' },
            body: postData
        });
        const responseData = await response.json();
        console.log('PHẢN HỒI TỪ SERVER:', responseData);
      

    } catch (error) {
        console.error('Lỗi khi gửi yêu cầu:', error);
    }
}


const smsToSend = [{
    "from": fromNumber,
    "to": toNumber,
    "text": "Message sent with new number!"
}];
sendSMS(smsToSend);


## Project description
JWT，全称Json Web Token，是一个开放标准(RFC 7519)，以JSON对象在各方安全地传输信息。
通俗来说，JWT是一个含签名并携带用户信息的Base64字符串，服务端在收到JWT串时，会进行验签匹配以保证信息未被篡改，验签通过则为合法身份


JWT由三部分构成，并由两个点分隔 header.playload .signature。
header部分：header部分指定token的类型和使用的算法（HMAC、SHA256、RSA等）
payload部分：playload是用来存用户信息(claims)的地方，claims有三种类型: registered、 public、private claims
	iss: jwt签发者
	sub: jwt所面向的用户
	aud: 接收jwt的一方
	exp: jwt的过期时间，这个过期时间必须要大于签发时间
	nbf: 定义在什么时间之前，该jwt都是不可用的.
	iat: jwt的签发时间
	jti: jwt的唯一身份标识，主要用来作为一次性token。
.signature部分：签名部分是由前两部分加securityKey组成
 

## nuget install
Microsoft.AspNetCore.Authentication.JwtBearer
Microsoft.IdentityModel.Tokens

Token存储在服务端Session中，在当前生命周期内有效，程序关闭则销毁
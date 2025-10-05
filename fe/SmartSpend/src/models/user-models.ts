export interface UserLoginDto
{
    Email: string;
    Password: string;
}

export interface UserBaseDto
{
    Username?: string;
    Email?: string;
    PhoneNumber?: string;
}

export interface UserRegisterDto extends UserBaseDto
{
    Password: string;
    ConfirmPassword: string;
}

export interface UserLoginResponseDto extends UserBaseDto
{
    Id: number;
    Token: string;
    RefreshToken: string;
    RefreshTokenExpiry: Date;
}

export interface User extends UserLoginResponseDto
{

}


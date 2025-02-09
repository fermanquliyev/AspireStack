export interface JwtPayload {
  sub: string;
  name: string;
  email: string;
  family_name: string;
  unique_name: string;
  auth_time: string;
  email_verified: string;
  role: string | string[];
  permission: string[];
  exp: number;
  iss: string;
  aud: string;
}

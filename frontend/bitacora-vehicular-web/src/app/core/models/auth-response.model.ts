export interface Authresponse {
  exito           : boolean;
  mensaje         : string;
  token?          : string | null;
  expiracion?     : string | null;
  usuarioId?      : number | null;
  nombreCompleto? : string | null;
  correo?         : string | null;
  nombreUsuario?  : string | null;
  rol?            : string | null;
  estado?         : string | null;
}

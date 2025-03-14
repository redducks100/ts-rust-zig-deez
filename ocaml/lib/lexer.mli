(* This is the public interface for the Lexer.
   `.ml` files contain the implementation
   `.mli` files contain the interface *)

(** All fields for the Lexer are hidden. You can have a lexer,
    but the available functions are defined below.

    This makes it impossible to create a new lexer except through init *)
type t

(** Create a new lexer from an input *)
val init : string -> t

(** Move the lexer to the next token.
    Returns the new lexer (lexers are immuntable) and an optional token *)
val next_token : t -> t * Token.t option

(** Formatter for pretty printing the lexer *)
val pp: Format.formatter -> t -> unit

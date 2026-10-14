# Reglas del proyecto

## 🔒 Claves de API de IA — REGLA OBLIGATORIA

Esta regla aplica a Claude, Claude Code, Cowork y a cualquier agente o persona que trabaje en este repo.

1. **NUNCA subir una clave de API de IA.** Esto incluye OpenAI (`sk-`, `sk-proj-`), Anthropic (`sk-ant-`),
   Google / Gemini (`AIza...`), Groq (`gsk_`), xAI, DeepSeek, OpenRouter, Hugging Face (`hf_`) y cualquier otro proveedor.
   No puede ir en commits, PRs, issues, artifacts, documentos, logs, capturas ni código del frontend.
2. **SIEMPRE AVISAR ANTES.** Si una acción puede subir o exponer una clave de IA (un `.env` sin ignorar,
   una clave escrita en el código, un log que la imprime, un archivo nuevo con secretos), **detente y avisa
   al dueño del repo antes de continuar**. No sigas sin confirmación explícita.
3. **Dónde sí van las claves:** solo en variables de entorno del servidor: el `.env` local (que debe estar en
   `.gitignore`) o el panel de secretos de Vercel, Supabase o GitHub. Nunca con prefijo `NEXT_PUBLIC_`/`VITE_`
   ni en código que llegue al navegador. En los `.env.example` el valor va vacío.
4. **Antes de cada commit o push**, revisa el diff buscando claves. Si encuentras una que ya se subió, avisa de
   inmediato: hay que **revocarla y crear una nueva** (borrar el commit no basta, porque sigue en el historial).
5. Lo mismo aplica a cualquier otro secreto: contraseñas SMTP, JWT secrets, cadenas de conexión a la base de datos
   y cuentas de servicio de Firebase/Google.

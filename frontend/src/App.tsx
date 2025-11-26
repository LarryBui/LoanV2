import { FormEvent, useMemo, useState } from 'react'
import './App.css'

type UserResponse = {
  id: string
  firstName: string
  lastName: string
  email: string
  phoneNumber: string
  createdAtUtc: string
}

type AuthResponse = {
  accessToken: string
  expiresAtUtc: string
  user: UserResponse
}

const fallbackBaseUrl = 'http://localhost:5080'

function App() {
  const [registerForm, setRegisterForm] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: '',
    password: '',
  })
  const [loginForm, setLoginForm] = useState({ email: '', password: '' })
  const [status, setStatus] = useState<string | null>(null)
  const [token, setToken] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)

  const baseUrl = useMemo(
    () => import.meta.env.VITE_API_BASE_URL?.toString() ?? fallbackBaseUrl,
    []
  )

  const handleRegister = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    submitAuth('/auth/register', registerForm)
  }

  const handleLogin = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    submitAuth('/auth/login', loginForm)
  }

  const submitAuth = async (path: string, payload: object) => {
    setLoading(true)
    setStatus(null)

    try {
      const response = await fetch(`${baseUrl}${path}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload),
      })

      const body = (await response.json().catch(() => ({}))) as
        | AuthResponse
        | { title?: string; errors?: Record<string, string[]> }

      if (!response.ok) {
        const fallbackError =
          'Something went wrong. Check your fields and try again.'
        const errors =
          'errors' in body && body.errors
            ? Object.values(body.errors).flat().join(', ')
            : undefined
        throw new Error(
          ('title' in body && body.title) || errors || fallbackError
        )
      }

      if ('accessToken' in body) {
        setToken(body.accessToken)
        setStatus(
          `Welcome back, ${body.user.firstName}! Token expires at ${new Date(
            body.expiresAtUtc
          ).toLocaleString()}`
        )
      } else {
        setStatus('Authentication succeeded.')
      }
    } catch (error) {
      const message =
        error instanceof Error ? error.message : 'Unexpected error occurred.'
      setStatus(message)
      setToken(null)
    } finally {
      setLoading(false)
    }
  }

  return (
    <main className="shell">
      <header className="hero">
        <div>
          <p className="eyebrow">OaigLoan API playground</p>
          <h1>Auth-ready starter</h1>
          <p className="lede">
            Register or sign in through the API. Responses echo the JWT issued
            by the backend API so you can wire the rest of the app quickly.
          </p>
          <p className="meta">
            Target API URL:{' '}
            <span className="code">{baseUrl.replace(/\/$/, '')}</span>
          </p>
        </div>
        <div className="pillars">
          <div className="pillar">CQRS</div>
          <div className="pillar">JWT</div>
          <div className="pillar">Outbox-ready</div>
        </div>
      </header>

      <section className="grid">
        <article className="panel">
          <div className="panel__header">
            <h2>Create account</h2>
            <p>Registers a user and emits an outbox event.</p>
          </div>
          <form className="stack" onSubmit={handleRegister}>
            <div className="field-row">
              <label>
                First name
                <input
                  required
                  value={registerForm.firstName}
                  onChange={(e) =>
                    setRegisterForm({ ...registerForm, firstName: e.target.value })
                  }
                  placeholder="Jane"
                />
              </label>
              <label>
                Last name
                <input
                  required
                  value={registerForm.lastName}
                  onChange={(e) =>
                    setRegisterForm({ ...registerForm, lastName: e.target.value })
                  }
                  placeholder="Doe"
                />
              </label>
            </div>
            <label>
              Email
              <input
                required
                type="email"
                value={registerForm.email}
                onChange={(e) =>
                  setRegisterForm({ ...registerForm, email: e.target.value })
                }
                placeholder="jane.doe@email.com"
              />
            </label>
            <label>
              Phone number
              <input
                required
                value={registerForm.phoneNumber}
                onChange={(e) =>
                  setRegisterForm({
                    ...registerForm,
                    phoneNumber: e.target.value,
                  })
                }
                placeholder="+1 555 0100"
              />
            </label>
            <label>
              Password
              <input
                required
                type="password"
                value={registerForm.password}
                onChange={(e) =>
                  setRegisterForm({ ...registerForm, password: e.target.value })
                }
                placeholder="At least 8 characters"
              />
            </label>
            <button type="submit" disabled={loading}>
              {loading ? 'Working...' : 'Register'}
            </button>
          </form>
        </article>

        <article className="panel">
          <div className="panel__header">
            <h2>Sign in</h2>
            <p>Authenticates the user and returns a JWT.</p>
          </div>
          <form className="stack" onSubmit={handleLogin}>
            <label>
              Email
              <input
                required
                type="email"
                value={loginForm.email}
                onChange={(e) =>
                  setLoginForm({ ...loginForm, email: e.target.value })
                }
                placeholder="registered@email.com"
              />
            </label>
            <label>
              Password
              <input
                required
                type="password"
                value={loginForm.password}
                onChange={(e) =>
                  setLoginForm({ ...loginForm, password: e.target.value })
                }
                placeholder="Your secret"
              />
            </label>
            <button type="submit" disabled={loading}>
              {loading ? 'Working...' : 'Sign in'}
            </button>
          </form>
        </article>
      </section>

      <section className="status">
        <div className="panel">
          <div className="panel__header">
            <h3>Result</h3>
            <p>Responses from the API are displayed here.</p>
          </div>
          <div className="result">
            {status ? <p>{status}</p> : <p>Run a request to see the output.</p>}
            {token ? (
              <div className="token">
                <p className="token__label">Access token</p>
                <code>{token}</code>
              </div>
            ) : null}
          </div>
        </div>
      </section>
    </main>
  )
}

export default App

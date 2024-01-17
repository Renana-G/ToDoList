import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom'

import Login from './components/login.js';
import SignUp from './components/signup.js';
import Todos from './todos.js';

function App() {

return (
    <Router>
      {/* <div className="App">
        <nav className="navbar navbar-expand-lg navbar-light fixed-top">
          <div className="container">
            <div className="collapse navbar-collapse" id="navbarTogglerDemo02">
              <ul className="navbar-nav ml-auto">
                <li className="nav-item">
                  <Link className="nav-link" to={'/sign-in'}>
                    Login
                  </Link>
                </li>
                <li className="nav-item">
                  <Link className="nav-link" to={'/sign-up'}>
                    Sign up
                  </Link>
                </li>
              </ul>
            </div>
          </div>
        </nav> */}

        <div className="auth-wrapper">
          <div className="auth-inner">
            <Routes>
              <Route exact path="/" element={<Todos />} />
              {/* <Route path="/sign-in" element={<Login />} />
              <Route path="/sign-up" element={<SignUp />} /> */}
            </Routes>
          </div>
        </div>
      {/* </div> */}
    </Router>
  )
}

export default App

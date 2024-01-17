import React, { Component, useState } from 'react'
import service from '../service';

function SignUp(){

    const [email, setEmail] = useState();
    const [password, setPassword] = useState();

    const signup = async()=>{
        console.log({email, password})
        service.register(email, password)

    }

    return (
      <form>
        <h3>Sign Up</h3>

        <div className="mb-3">
          <label>Email address</label>
          <input
            type="email"
            className="form-control"
            placeholder="Enter email"
            onChange={(event)=>setEmail(event.target.value)}
          />
        </div>

        <div className="mb-3">
          <label>Password</label>
          <input
            type="password"
            className="form-control"
            placeholder="Enter password"
            onChange={(event)=>setPassword(event.target.value)}
          />
        </div>

        <div className="d-grid">
          <button type="submit" className="btn btn-primary" onClick={signup}>
            Sign Up
          </button>
        </div>
        <p className="forgot-password text-right">
          Already registered <a href="/sign-in">sign in?</a>
        </p>
      </form>
    )
  }

export default SignUp;
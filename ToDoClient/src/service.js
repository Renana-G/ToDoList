import axios from 'axios';

// const apiUrl = "http://localhost:5158"
// axios.defaults.baseURL = "http://localhost:5158"
axios.defaults.baseURL = process.env.REACT_APP_DEFAULT_URL


axios.interceptors.response.use(
  (response) => {
    // Log the response information or perform any other action
    console.log("Success")
    return response
  },
  (error) => {
    // Handle response error
    if(error.response.status == 401)
     {
      window.location.href = "/sign-up"
     } 
    
    return error
  }
);


// axios.defaults.headers.post['Content-Type'] = 'application/x-www-form-urlencoded';


export default {
  setAccessToken: ()=>{
    const token = localStorage.getItem("token")
    if(token)
      axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
  },  

  login: async(email, password)=>{
    const result = await axios.post("/login", {email: email, password: password})
    if(result.data){
      localStorage.setItem("token", `Bearer ${result.data}`);
      axios.defaults.headers.common['Authorization'] = `Bearer ${result.data}`;
      return result.data;
    }
  },

  register: async(email, password)=>{
    const result = await axios.post("/register", {email: email, password: password})
    return result
  },

  getTasks: async () => {
      const result = await axios.get("/task")
      console.log(result.data);
      debugger;
      return result.data || [];
  },

  addTask: async (name) => {
      const result = await axios.post(`/task`, { name: name })
      return result.data || [];
  },

  setCompleted: async (id, isComplete) => {
      const result = await axios.put(`/task/${id}`, { isComplete: isComplete })
      console.log('setCompleted', { id, isComplete })
      return result.data || {};
  },

  deleteTask: async (id) => {
  
      const result = await axios.delete(`/task/${id}`)
      console.log('deleteTask')
      return result.data || {};
  }
};

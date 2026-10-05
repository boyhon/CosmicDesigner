document.addEventListener("DOMContentLoaded",()=>{
  const cosmic=document.body.classList.contains("cosmic-manual");
  document.querySelectorAll(".nav a").forEach(a=>{
    if(new URL(a.getAttribute("href"),location.href).pathname===location.pathname)a.classList.add("active");
  });
  const search=document.getElementById("nav-search");
  if(search)search.addEventListener("input",()=>{
    const term=search.value.trim().toLocaleLowerCase();
    document.querySelectorAll("#manual-links a").forEach(a=>a.hidden=!a.textContent.toLocaleLowerCase().includes(term));
  });
  document.addEventListener("keydown",e=>{
    if(e.key==="Home"&&e.altKey){
      e.preventDefault();
      location.href=cosmic?document.querySelector('.nav a[href$="index.html"]').href:"../DXFExplorer_help.html";
    }
  });
});

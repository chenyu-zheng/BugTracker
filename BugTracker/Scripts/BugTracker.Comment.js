document.addEventListener('DOMContentLoaded', () => {
  const createBtn = document.querySelector('#CreateComment');
  const createForm = document.querySelector('#CreateForm');
  const formError = document.querySelector('#CreateFormError');
  const commentList = document.querySelector('#CommentList');
  const template = document.querySelector('#listItem');

  createBtn.addEventListener('click', () => {
    switchForm();
  });

  createForm.addEventListener('submit', e => {
    e.preventDefault();
    const formData = new FormData(e.target);

    postComment(formData, hendleCommentResponse);
  });

  getCommentList(renderList);

  function renderList(json) {
    while (commentList.firstChild) {
      commentList.removeChild(commentList.firstChild);
    }
    json.data.Comments.forEach(item => {
      const clone = document.importNode(template.content, true);
      clone.querySelector('.comment-author').textContent =
        item.AuthorDisplayName;
      clone.querySelector('.comment-updated').textContent = item.Updated
        ? 'Edited ' + item.Updated
        : '';
      clone.querySelector('.comment-created').textContent =
        'Posted ' + item.Created;
      clone.querySelector('.comment-content').textContent = item.Content;
      commentList.appendChild(clone);
    });
  }

  function switchForm() {
    if (createForm.style['display'] == 'none') {
      createBtn.innerHTML = `<i class="fas fa-ban fa-fw"></i> Cancel`;
      createForm.style['display'] = 'block';
    } else {
      createBtn.innerHTML = `<i class="fas fa-pen fa-fw"></i> New`;
      createForm.style['display'] = 'none';
    }
  }

  function showFormError(error) {
    formError.textContent = error;
  }

  function hendleCommentResponse(json) {
    if (json.error) {
      showFormError(json.error);
    } else if (json.success) {
      createForm.Content.value = '';
      switchForm();
      getCommentList(renderList);
    } else {
      console.log('Unhandled error');
      console.log(json);
    }
  }

  function getCommentList(callback) {
    fetch(commentListUrl + '?ticketId=' + ticketId, {
      headers: { 'Content-Type': 'application/json' },
      credentials: 'same-origin'
    })
      .then(response => {
        if (!response.ok) {
          throw response;
        }
        return response.json();
      })
      .then(json => callback(json))
      .catch(response => console.log(response));
  }

  function postComment(data, callback) {
    fetch(submitCommentUrl, {
      method: 'POST',
      credentials: 'same-origin',
      body: data
    })
      .then(response => {
        if (!response.ok) {
          throw response;
        }
        return response.json();
      })
      .then(json => callback(json))
      .catch(response => console.log(response));
  }
});

document.addEventListener('DOMContentLoaded', () => {
  const createBtn = document.querySelector('#CreateComment');
  const createForm = document.querySelector('#CreateForm');
  const formError = document.querySelector('#CreateFormError');
  const commentList = document.querySelector('#CommentList');
  const commentTemplate = document.querySelector('#listItem');

  createBtn.addEventListener('click', () => {
    switchCreateForm();
  });

  createForm.addEventListener('submit', e => {
    e.preventDefault();
    const formData = new FormData(e.target);
    postComment(formData).then(json => {
      if (json.error) {
        formError.textContent = error;
      } else if (json.success) {
        createForm.Content.value = '';
        switchCreateForm();
        refreshComments();
      } else {
        console.error('Unhandled error');
        console.log(json);
      }
    });
  });

  refreshComments();

  function refreshComments() {
    getCommentList(ticketId).then(json => renderList(json));
  }

  function renderList(json) {
    while (commentList.firstChild) {
      commentList.removeChild(commentList.firstChild);
    }
    json.data.Comments.forEach(item => {
      const clone = document.importNode(commentTemplate.content, true);
      clone.querySelector('.comment-author').textContent =
        item.AuthorDisplayName;
      clone.querySelector('.comment-updated').textContent = item.Updated
        ? 'Edited ' + item.Updated
        : '';
      clone.querySelector('.comment-created').textContent =
        'Posted ' + item.Created;
      clone.querySelector('.comment-content').textContent = item.Content;
      commentList.appendChild(clone);

      if (item.CanEdit) {
        const comment = commentList.lastElementChild;
        comment.querySelector('.buttons').style['display'] = 'block';

        comment
          .querySelector('.delete-button')
          .addEventListener('click', () => switchDeleteForm(comment));
        const deleteForm = comment.querySelector('.delete-form');
        deleteForm
          .querySelector('.cancel')
          .addEventListener('click', () => switchDeleteForm(comment));
        deleteForm.addEventListener('submit', e => submitDelete(e, item.Id));

        comment
          .querySelector('.edit-button')
          .addEventListener('click', () => switchEditForm(comment));
        const editForm = comment.querySelector('.edit-form');
        editForm.Content.value = item.Content;
        editForm
          .querySelector('.cancel')
          .addEventListener('click', () => switchEditForm(comment));
        editForm.addEventListener('submit', e => submitEdit(e, item.Id));
      }
    });
  }

  function switchEditForm(commentElement) {
    const form = commentElement.querySelector('.edit-form');
    const buttons = commentElement.querySelector('.buttons');
    const content = commentElement.querySelector('.comment-content');
    if (form.style['display'] == 'none') {
      form.style['display'] = 'block';
      buttons.style['display'] = 'none';
      content.style['display'] = 'none';
    } else {
      form.style['display'] = 'none';
      buttons.style['display'] = 'block';
      content.style['display'] = 'block';
    }
  }

  function submitEdit(e, commentId) {
    e.preventDefault();
    const formData = new FormData(e.target);
    formData.append(
      '__RequestVerificationToken',
      createForm['__RequestVerificationToken'].value
    );
    formData.append('Id', commentId);
    editComment(commentId, formData).then(json => {
      if (json.error) {
        form.querySelector('.edit-form-error').textContent = error;
      } else if (json.success) {
        refreshComments();
      } else {
        console.error('Unhandled error');
        console.log(json);
      }
    });
  }

  function switchDeleteForm(commentElement) {
    const form = commentElement.querySelector('.delete-form');
    const buttons = commentElement.querySelector('.buttons');
    if (form.style['display'] == 'none') {
      form.style['display'] = 'block';
      buttons.style['display'] = 'none';
    } else {
      form.style['display'] = 'none';
      buttons.style['display'] = 'block';
    }
  }

  function submitDelete(e, commentId) {
    e.preventDefault();
    const formData = new FormData(e.target);
    formData.append(
      '__RequestVerificationToken',
      createForm['__RequestVerificationToken'].value
    );
    deleteComment(commentId, formData).then(refreshComments);
  }

  function switchCreateForm() {
    if (createForm.style['display'] == 'none') {
      createBtn.innerHTML = `<i class="fas fa-ban fa-fw"></i> Cancel`;
      createForm.style['display'] = 'block';
    } else {
      createBtn.innerHTML = `<i class="fas fa-pen fa-fw"></i> New`;
      createForm.style['display'] = 'none';
    }
  }

  function handleResponse(json) {
    if (json.error) {
      console.error(json.error);
    }
  }

  function getCommentList(ticketId) {
    return fetch(commentListUrl + '?ticketId=' + ticketId, {
      headers: { 'Content-Type': 'application/json' },
      credentials: 'same-origin'
    })
      .then(response => {
        if (!response.ok) {
          throw response;
        }
        return response.json();
      })
      .then(json => {
        handleResponse(json);
        return json;
      })
      .catch(response => console.log(response));
  }

  function postComment(formData) {
    return fetch(submitCommentUrl, {
      method: 'POST',
      credentials: 'same-origin',
      body: formData
    })
      .then(response => {
        if (!response.ok) {
          throw response;
        }
        return response.json();
      })
      .catch(response => console.log(response));
  }

  function editComment(id, formData) {
    return fetch(editCommentUrl + '?id=' + id, {
      method: 'POST',
      credentials: 'same-origin',
      body: formData
    })
      .then(response => {
        if (!response.ok) {
          throw response;
        }
        return response.json();
      })
      .then(json => {
        handleResponse(json);
        return json;
      })
      .catch(response => console.log(response));
  }

  function deleteComment(id, formData) {
    return fetch(deleteCommentUrl + '?id=' + id, {
      method: 'POST',
      credentials: 'same-origin',
      body: formData
    })
      .then(response => {
        if (!response.ok) {
          throw response;
        }
        return response.json();
      })
      .then(json => {
        handleResponse(json);
        return json;
      })
      .catch(response => console.log(response));
  }
});

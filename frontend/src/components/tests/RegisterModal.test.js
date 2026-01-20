import { mount } from '@vue/test-utils'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import RegisterModal from './RegisterModal.vue'

describe('LoginModal.vue', () => {
  global.fetch = vi.fn()

  beforeEach(() => {
    vi.clearAllMocks()
    document.body.innerHTML = ''
  })

  it('shows validation errors for empty fields on submit', async () => {
    const wrapper = mount(RegisterModal, {
      props: { isOpen: true },
      attachTo: document.body
    })

    const form = document.querySelector('form')
    form.dispatchEvent(new Event('submit'))

    await new Promise(resolve => setTimeout(resolve, 50))

    expect(document.body.innerHTML).toContain('Email required')
    
    wrapper.unmount()
  })

  it('emits close event when X button is clicked', async () => {
    const wrapper = mount(RegisterModal, {
      props: { isOpen: true },
      attachTo: document.body
    })

    const closeBtn = document.querySelector('#cancelButton')
    closeBtn.click()

    expect(wrapper.emitted()).toHaveProperty('close')
    
    wrapper.unmount()
  })
})
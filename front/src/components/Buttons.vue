<template>
    <button :class="buttonClass" @click="$emit('click')"><slot></slot></button>
</template>

<script>
import { useRouter } from 'vue-router'

export default {
    name: 'TheButton',
    props: {
        variant: {
        type: String,
        default: 'primary',
        validator: (value) => [
            'primary',
            'secondary'
        ].includes(value)
        },
        to: {
        type: [String, Object],
        default: null
        }
    },
    setup(props) {
        const router = useRouter()
        const handleClick = () => {
            if (props.to) {
                router.push(props.to)
            }
        }
    },
    computed: {
        buttonClass() {
            return [
                'button',
                `button--${this.variant}`
            ] 
        }
  }
}
</script>

<style scoped>
    .button {
        font-size: var(--buttonfontsize);
        letter-spacing: 0.06rem;
        border: 0.1em solid;
        border-color: var(--main-bg-color);
        border-radius:0.6em;
        cursor: pointer;
        transition: all 0.3s ease;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        padding:0.9em 1.1em;
        font-weight: bold;
    }

    .button--primary {
        background-color: var(--main-bg-color);
        color: var(--color-dark-pink);
    }

    .button--secondary {
        background-color: var(--color-dark-pink);
        color: var(--main-bg-color);
    }

    .button:hover {
        opacity: 0.9;
        transform: translateY(-5px);
    }
</style>
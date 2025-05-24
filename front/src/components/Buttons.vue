<template>
    <button :class="buttonClass" @click="handleClick"><slot></slot></button>
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
        return { handleClick }
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
        font-size: var(--h2size);
        letter-spacing: 0.06rem;
        border: 0.1em solid;
        border-color: var(--main-bg-color);
        border-radius: var(--borderradius);
        cursor: pointer;
        transition: all 0.3s ease;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        padding:0.9em 1.1em;
        font-weight: var(--h2weight);
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